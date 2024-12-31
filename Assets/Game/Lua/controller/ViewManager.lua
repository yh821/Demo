---
--- Created by Hugo
--- DateTime: 2023/5/5 16:40
---

---@class ViewManager : BaseClass
ViewManager = ViewManager or BaseClass()

UiLayer = GameObject.Find("GameRoot/UiLayer")

function ViewManager:__init()
    if ViewManager.Instance then
        print_error("[ViewManager] attempt to create singleton twice!")
        return
    end
    ViewManager.Instance = self

    ---@type BaseView[]
    self._open_view_list = {}
    ---@type BaseView[]
    self._view_list = {}
end

function ViewManager:__delete()
    ViewManager.Instance = nil
end

function ViewManager:AddView(view, view_name)
    if IsNilOrEmpty(view_name) then
        print_error("[ViewManager] 请指定view name")
        return
    end
    self._view_list[view_name] = view
end

function ViewManager:RemoveView(view_name)
    self._view_list[view_name] = nil
end

function ViewManager:GetView(view_name)
    return self._view_list[view_name]
end

function ViewManager:IsOpen(view_name)
    local view = self:GetView(view_name)
    if view then
        return view:IsOpen()
    end
end

function ViewManager:OpenView(view_name, callback)
    local view = self._open_view_list[view_name]
    if view then
        view:OnFlush()
        return
    end

    local bundle_name = string.lower("uis/view/" .. view_name .. "_prefab")
    local asset_name = view_name .. "View"
    local prefab = EditorResourceMgr.LoadGameObject(bundle_name, asset_name)
    local go = ResMgr.Instance:Instantiate(prefab)
    if IsNil(go) then
        return
    end
    local trans = go.transform
    trans:SetParent(UiLayer.transform, false)
    trans.localPosition = Vector3Pool.GetTemp(0, 0, 0)
    go.name = view_name
    if callback then
        callback(go)
    end
end

function ViewManager:Open(view_name, tab_index)
    local view = self._view_list[view_name]
    if not view then
        return false
    end

    local index = tonumber(tab_index)
    if index == nil and not IsNilOrEmpty(tab_index) then
        index = TabIndexMap[tab_index]
    end

    local is_open, msg = SysOpen.Instance:GetSysIsOpened(view_name)
    if not is_open then
        MessageCtrl.Instance:ShowMessage(msg)
        return false
    end
end

function ViewManager:Close(view_name, ...)
    local view = self:GetView(view_name)
    if view then
        view:Close(...)
    end
end

function ViewManager:CloseAll()
    for k, v in pairs(self._view_list) do
        if v:CanActiveClose() then
            if v:IsOpen() then
                v:Close()
            end
        end
    end
end

function ViewManager:CloseAllLayer(layer)
    for k, v in pairs(self._view_list) do
        if v.view_layer == layer and v:CanActiveClose() then
            if v:IsOpen() then
                v:Close()
            end
        end
    end
end

function ViewManager:Invoke(view_name, method, ...)
    local view = self:GetView(view_name)
    if view and view:IsOpen() then
        local typo = type(method)
        if typo == "function" then
            return method(view, ...)
        elseif typo == "string" then
            local func = view[method]
            if func then
                return func(view, ...)
            else
                print_error(view_name .. ":" .. method .. " 方法不存在")
            end
        end
    end
end
