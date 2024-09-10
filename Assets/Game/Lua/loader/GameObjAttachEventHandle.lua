---
--- Created by Hugo
--- DateTime: 2024/2/18 17:47
---

---@class GameObjAttachEventHandle
GameObjAttachEventHandle = {}
GameObjAttachEventHandle._class_type = true

local function LoadCallBack(obj, game_obj_attach)
    if not IsNil(game_obj_attach) then
        game_obj_attach:OnLoadComplete(obj)
    end
end

function GameObjAttachEventHandle.EnabledGameObjAttachEvent(game_obj_attach_list)
    for i = 0, game_obj_attach_list.Count - 1 do
        local game_obj_attach = game_obj_attach_list[i]
        if not IsNil(game_obj_attach) then
            local bundle_name, asset_name = game_obj_attach.BundleName, game_obj_attach.AssetName
            if not IsNilOrEmpty(bundle_name) and not IsNilOrEmpty(asset_name) then
                local loader = GameObjAttachEventHandle.AllocLoader(game_obj_attach)
                loader:SetLoadPriority(ResLoadPriority.low)
                loader:SetIsUseObjPool(true)
                loader:Load(bundle_name, asset_name, LoadCallBack, game_obj_attach)
            end
        end
    end
end

local function DestroyLoader(game_obj_attach)
    if game_obj_attach ~= nil and GameObjAttachEventHandle.__game_obj_loaders then
        local loader_key = "id_" .. game_obj_attach:GetInstanceID()
        local loader = GameObjAttachEventHandle.__game_obj_loaders[loader_key]
        if loader then
            loader:Destroy()
        end
    end
end

function GameObjAttachEventHandle.DisabledGameObjAttachEvent(game_obj_attach_list)
    for i = 0, game_obj_attach_list.Count - 1 do
        DestroyLoader(game_obj_attach_list[i])
    end
end

function GameObjAttachEventHandle.DisabledGameObjAttachArray(game_obj_attach_list)
    for i = 0, game_obj_attach_list.Length - 1 do
        DestroyLoader(game_obj_attach_list[i])
    end
end

function GameObjAttachEventHandle.DestroyGameObjAttachEvent(instance_id_list)
    for i = 0, instance_id_list.Count - 1 do
        LoadUtil.DelGameObjLoader(GameObjAttachEventHandle, "id_" .. instance_id_list[i])
    end
end

function GameObjAttachEventHandle.AllocLoader(game_obj_attach)
    local loader = LoadUtil.AllocAsyncLoader(GameObjAttachEventHandle, "id_" .. game_obj_attach:GetInstanceID())
    loader:SetParent(game_obj_attach.transform)
    return loader
end