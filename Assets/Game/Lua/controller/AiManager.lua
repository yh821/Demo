﻿---
--- Created by Hugo
--- DateTime: 2023/4/4 22:03
---

require("behavior/BehaviorManager")

---@class AiManager : BaseClass
---@field _bt_list BehaviorTree[]
AiManager = AiManager or BaseClass()

function AiManager:__init()
    if AiManager.Instance then
        print_error("[AiManager] attempt to create singleton twice!")
        return
    end
    AiManager.Instance = self

    self._bt_list = {}

    Runner.Instance:AddRunObj(self, RunnerPriority.mid)
end

function AiManager:__delete()
    Runner.Instance:RemoveRunObj(self)

    self._bt_list = nil

    AiManager.Instance = nil
end

function AiManager:Update(now_time, delta_time)
    BehaviorManager:Update(delta_time)
end

function AiManager:SwitchTick(thinking)
    BehaviorManager:SwitchTick(thinking)
end

---@param scene_obj SceneObj
---@return BehaviorTree 一个实体只能绑定一个行为树
function AiManager:BindBT(scene_obj, file)
    local bt = BehaviorManager:BindBehaviorTree(scene_obj, file)
    if not bt then
        return
    end
    bt:SetSharedVar(BtConfig.self_obj_key, scene_obj)
    self._bt_list[scene_obj] = bt
    return bt
end

---@param scene_obj SceneObj
function AiManager:UnBindBT(scene_obj)
    BehaviorManager:UnBindBehaviorTree(scene_obj)
    self._bt_list[scene_obj] = nil
end
