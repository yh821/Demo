---
--- Created by Hugo
--- DateTime: 2024/10/9 10:25
---

--子面板
---@class BaseWidget : BaseClass
BaseWidget = BaseWidget or BaseClass()

function BaseWidget:__init(instance)
    self.active = true
end

function BaseWidget:__delete()

end

function BaseWidget:GetView()
    return self.view
end

function BaseWidget:LoadAsset(bundle, asset, parent, callback)
    self.async_loader = LoadUtil.AllocSyncLoader(self, "panel_view")
    self.async_loader:SetParent(parent)
    self.async_loader:Load(bundle, asset, function(obj)
        if IsNil(obj) then
            return
        end
        self:SetInstance(obj)
        if callback then
            callback(obj)
        end
    end)
end

function BaseWidget:SetInstance(instance)
    local typo = type(instance)
    if typo == "userdata" then
        self.view = U3DObject(instance)
    elseif typo == "table" and not instance.gameObject or typo == "boolean" then
        return
    else
        self.view = instance
    end

    self.name_table = instance:GetComponent(typeof(UINameTable))
    self.node_list = U3DNodeList(self.name_table, self)
    self:LoadCallBack(instance)
    if not self.active then
        self:SetActive(self.active)
    end
    self.is_loaded = true
    self:FlushHelper()
end

function BaseWidget:SetInstanceParent(instance)
    self.view.transform:SetParent(instance.transform, false)
end

function BaseWidget:SetLocalPosition(x, y, z)
    self.view.transform:SetLocalPosition(x, y, z)
end

local default_params = { "all" }
function BaseWidget:Flush(key, params)
    key = key or "all"
    params = params or default_params
    self.flush_params = self.flush_params or {}
    self.flush_params[key] = self.flush_params[key] or {}
    for k, v in pairs(params) do
        self.flush_params[key][k] = v
    end

    if not self.view and self.delay_flush_timer then
        self:FlushHelper()
    end
end

function BaseWidget:FlushHelper()
    self:CancelDelayFlushTimer()
    if not self.view then
        return
    end
    if not self.is_loaded then
        return
    end

    if self.flush_params then
        local params = self.flush_params
        self.flush_params = nil
        self:OnFlush(params)
        if self.need_change then
            self.need_change = false
            self:OnSelectChange(self.is_select)
        end
    end
end

function BaseWidget:CancelDelayFlushTimer()
    if self.delay_flush_timer then
        TimerQuest.Instance:CancelQuest(self.delay_flush_timer)
        self.delay_flush_timer = nil
    end
end

function BaseWidget:LoadSprite(bundle, asset, callback, cb_data)
    LoadUtil.LoadSprite(self, bundle, asset, callback, cb_data)
end

function BaseWidget:LoadSpriteAsync(bundle, asset, callback, cb_data)
    LoadUtil.LoadSpriteAsync(self, bundle, asset, callback, cb_data)
end

function BaseWidget:LoadRawImage(bundle, asset, callback)
    LoadUtil.LoadRawImage(self, bundle, asset, callback)
end

function BaseWidget:SetActive(value)
    if self.view and not IsNil(self.view.gameObject) then
        self.view.gameObject:SetActive(value)
    end
    self.active = value
end

function BaseWidget:SetParentActive(value)
    self.view.transform.parent.gameObject:SetActive(value)
end

function BaseWidget:IsNil()
    if not self.view or not self.view.gameObject then
        return true
    end
    return IsNil(self.view.gameObject)
end

function BaseWidget:IsOpen()
    return self.view and true or false
end

--------------------------------override begin--------------------------------

function BaseWidget:LoadCallBack(instance)
    --override
end

function BaseWidget:OnFlush(params)
    --override
end

function BaseWidget:CloseCallBack()
    --override
end

---------------------------------override end---------------------------------
