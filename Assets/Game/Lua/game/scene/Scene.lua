---
--- Created by Hugo
--- DateTime: 2023/5/6 17:43
---

require("core/DrawPart")
require("core/DrawObj")

require("game/scene/ResPath")
require("game/scene/StateMachine")

require("game/scene/scenelogic/SceneLogic")

require("game/scene/trigger/ActorCtrl")
require("game/scene/trigger/ActorTrigger")

require("game/scene/sceneobj/SceneObj")
require("game/scene/sceneobj/Character")
require("game/scene/sceneobj/Role")
require("game/scene/sceneobj/MainRole")
require("game/scene/sceneobj/Monster")
require("game/scene/sceneobj/Pet")

local GameRootTransform = GameObject.Find("GameRoot").transform

---@class Scene : BaseController
---@field obj_list SceneObj[]
Scene = Scene or BaseClass(BaseController)

function Scene:__init()
    if Scene.Instance then
        print_error("[Scene] attempt to create singleton twice!")
        return
    end
    Scene.Instance = self

    self.obj_list = {}
    self.obj_group_list = {}
    self.instance_id_list = {}

    self:RegisterEvents() --监听事件

    Runner.Instance:AddRunObj(self, RunnerPriority.slower)
end

function Scene:__delete()
    Runner.Instance:RemoveRunObj(self)

    self.obj_list = nil
    self.instance_id_list = nil
    self:ClearScene()

    Scene.Instance = nil
end

function Scene:RegisterEvents()
    self:Bind(SceneEventType.SCENE_START_LOAD, BindTool.Bind(self.OnChangeScene, self))
    self:Bind(SceneEventType.SCENE_LOAD_COMPLETE, BindTool.Bind1(self.OnLoadAssetEnd, self))
end

function Scene:CreateMainRole()
    local vo = { pos_x = 0, pos_y = 0, pos_z = 0, name = "MainRole" }
    self.main_role = self:CreateObj(vo, SceneObjType.MainRole)
end

---@return MainRole
function Scene:GetMainRole()
    return self.main_role
end

---@return Role
function Scene:CreateRole(vo)
    return self:CreateObj(vo, SceneObjType.Role)
end

---@return Pet
function Scene:CreatePet(vo)
    return self:CreateObj(vo, SceneObjType.Pet)
end

---@return Monster
function Scene:CreateMonster(vo)
    return self:CreateObj(vo, SceneObjType.Monster)
end

local client_obj_id = 0x10000
function Scene:GetSceneClientId()
    client_obj_id = client_obj_id + 1
    return client_obj_id
end

function Scene:CreateObj(vo, obj_type)
    if vo.obj_id == nil or vo.obj_id < 0 then
        vo.obj_id = self:GetSceneClientId()
    end
    local obj_id = vo.obj_id
    local old_obj = self.obj_list[obj_id]
    if old_obj then
        print_error("[Scene] obj exits in obj_list", old_obj:GetType(), vo.name, obj_type)
        return
    end
    ---@type SceneObj
    local obj
    if obj_type == SceneObjType.Role then
        obj = Role.New(vo)
    elseif obj_type == SceneObjType.MainRole then
        obj = MainRole.New(vo)
    elseif obj_type == SceneObjType.Pet then
        obj = Pet.New(vo)
    elseif obj_type == SceneObjType.Monster then
        obj = Monster.New(vo)
    end
    self.obj_list[obj_id] = obj
    self.instance_id_list[obj.draw_obj:GetTransform():GetInstanceID()] = obj_id
    local obj_group = self.obj_group_list[obj_type] or {}
    self.obj_group_list[obj_type] = obj_group
    obj_group[obj_id] = obj

    obj.draw_obj:SetObjType(obj_type)
    obj:Init(self)

    self:Fire(ObjectEventType.CREATE_OBJ, obj)

    return obj
end

function Scene:DeleteObj(obj_id, delay)
    local obj = self.obj_list[obj_id]
    if obj == nil then
        return
    end
    if obj == self.main_role then
        return
    end
    delay = delay or 0
    for k, v in pairs(self.instance_id_list) do
        if obj_id == v then
            self.instance_id_list[k] = nil
        end
    end

    self.obj_list[obj_id] = nil
    local obj_group = self:GetObjListByType(obj:GetType())
    if obj == obj_group[obj_id] then
        obj_group[obj_id] = nil
    end

    AiManager.Instance:UnBindBT(obj)

    self:Fire(ObjectEventType.DELETE_OBJ, obj)

    if delay > 0 then
        TimerQuest.Instance:AddDelayTimer(function()
            obj:DeleteMe()
        end, delay)
    else
        obj:DeleteMe()
    end
end

function Scene:Update(now_time, delta_time)
    for _, v in pairs(self.obj_list) do
        v:Update(now_time, delta_time)
    end
end

---@return SceneObj[]
function Scene:GetObjList()
    return self.obj_list or EmptyTable
end

---@return SceneObj[]
function Scene:GetObjListByType(obj_type)
    return self.obj_group_list[obj_type] or EmptyTable
end

function Scene:GetSceneObj(obj_id)
    if obj_id and self.obj_list then
        return self.obj_list[obj_id]
    end
end

function Scene:DeleteAllObj()
    for k, v in pairs(self.obj_list) do
        self.obj_list[k] = nil
        v:DeleteMe()
    end
end

function Scene:DeleteObjListByType(obj_type)
    local obj_group = self:GetObjListByType(obj_type)
    for _, v in pairs(obj_group) do
        self:DeleteObj(v:GetObjId())
    end
end

function Scene:CreateCamera()
    if MainCamera == nil then
        --self.camera_obj = ResPoolMgr.Instance:TryGetGameObject("scenes_prefab", "Camera")
        self.camera_obj = GameObject.Find("GameRoot/Camera")
        self.camera_obj.transform:SetParent(GameRootTransform, false)
        MainCamera = self.camera_obj:GetComponent(typeof(SimpleCamera))
    end
end

function Scene:OnChangeScene(scene_id)
    local scene_cfg = ConfigManager.Instance:GetSceneConfig(scene_id)
    if not scene_cfg then
        print_error("can not find scene config, scene id:" .. scene_id)
        return
    end

    if self.old_scene_id then
        self.old_scene_id = self:GetSceneId()
    else
        self.old_scene_id = -1
    end

    self.old_scene_type = self.scene_cfg and self.scene_cfg.scene_type or -1
    if self.scene_logic then
        self.scene_logic:Out(self.old_scene_type, scene_cfg.scene_type)
    end

    local is_same_asset = false --是否同资源场景
    --上个场景配置
    if self.scene_cfg and self.scene_cfg.bundle_name == scene_cfg.bundle_name and self.scene_cfg.asset_name == scene_cfg.asset_name then
        is_same_asset = true
    end
    self.scene_cfg = scene_cfg

    self:ClearScene()

    self.scene_logic = SceneLogic.Create(scene_cfg.scene_type, scene_id)
    if is_same_asset then
        if not sel:LoadingViewIsOpen() then
            self:LoadSameScene(scene_id, scene_cfg.scene_type)
        end
        self:ResetMainRole()
    else
        self:StartLoadScene(scene_id)
    end
end

function Scene:GetSceneId()
    return self.scene_cfg and self.scene_cfg.id or 0
end

function Scene:GetOldSceneId()
    return self.old_scene_id
end

function Scene:ClearScene()
    self:DeleteAllObj()

    if self.scene_logic then
        self.scene_logic:DeleteMe()
        self.scene_logic = nil
    end
end

function Scene:LoadSameScene(scene_id, scene_type)
    self.start_loading_time = Status.NowTime
    self.asset_scene_id = scene_id
    self.OnLoadSceneMainComplete(scene_id, scene_type)
    self:OnLoadAssetEnd(scene_id, true)
end

function Scene:StartLoadScene(scene_id)
    if not self:IsSceneLoading() and self.asset_scene_id == scene_id then
        self.OnLoadSceneMainComplete(scene_id)
        --self.OnLoadSceneDetailComplete(scene_id)
        return
    end
    self.loadingCtrl:StartLoad(scene_id)
end

function Scene:OnLoadStart(scene_id)
    self.isLoading = true
    self.start_loading_time = Status.NowTime
    --ViewManager.Instance:Close(ModuleName.Login)
end

function Scene:OnLoadMainEnd(scene_id)
    if self.camera then
        self.camera:ResetParameters()
    end
    self.asset_scene_id = scene_id
    self.IsChangingScene(false)

    self:CreateCamera()
    self:OnLoadSceneMainComplete(scene_id)
end

function Scene:OnLoadAssetEnd(scene_id, is_same_asset)
    local loading_time = Status.NowTime - self.start_loading_time
    print_log("[Scene] OnLoadAssetEnd", scene_id, loading_time)
    self.asset_scene_id = scene_id
    --self:OnLoadSceneDetailComplete(scene_id)
    self:Fire(SceneEventType.SCENE_LOAD_ASSET_COMPLETE)
end

function Scene:IsSceneLoading()
    return self.isLoading == true
end

function Scene:OnLoadSceneMainComplete(scene_id,scene_type)
    self.isLoading=false
    --self:SceneObjLoadSceneComplete()
    self:Fire(SceneEventType.SCENE_LOAD_MAIN_COMPLETE)
end