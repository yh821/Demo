---
--- Created by Hugo
--- DateTime: 2024/10/8 11:10
---

---@class BaseSceneLogic : BaseClass
BaseSceneLogic = BaseSceneLogic or BaseClass()

function BaseSceneLogic:__init()
end

function BaseSceneLogic:__delete()
end

function BaseSceneLogic:SetSceneType(scene_type)
    self.scene_type = scene_type
end
