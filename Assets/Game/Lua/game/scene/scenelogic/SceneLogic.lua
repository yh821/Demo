---
--- Created by Hugo
--- DateTime: 2024/10/8 11:08
---

SceneType = {
    Common = 0, --普通场景
    BossFb = 1, --boss副本
    PetFb = 2, --宠物副本
}

require("game/scene/scenelogic/BaseSceneLogic")
require("game/scene/scenelogic/BaseFbSceneLogic")

require("game/scene/scenelogic/CommonSceneLogic")
require("game/scene/scenelogic/BossFbSceneLogic")
require("game/scene/scenelogic/PetFbSceneLogic")

---@class SceneLogic
SceneLogic = SceneLogic or {}

function SceneLogic.Create(scene_type)
    ---@type BaseSceneLogic
    local scene_logic
    if scene_type == SceneType.BossFb then
        scene_logic = BossFbSceneLogic.New()
    elseif scene_type == SceneType.PetFb then
        scene_logic = PetFbSceneLogic.New()
    elseif scene_type == SceneType.Common then
        scene_logic = CommonSceneLogic.New()
    else
        scene_logic = BaseSceneLogic.New()
    end

    if scene_logic then
        scene_logic:SetSceneType(scene_type)
    end
    return scene_logic
end