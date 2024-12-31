---
--- Created by Hugo
--- DateTime: 2024/10/8 10:21
---

require("game/scene/loading/LoadingView")

---@class LoadingCtrl
---@field view LoadingView
LoadingCtrl = LoadingCtrl or BaseClass()

local loadState = {
    LoadBundle = 1,
    CompleteBundle = 2,
    LoadScene = 3,
    Complete = 4,
}

function LoadingCtrl:__init()
    self.view = LoadingView.New()
    self.state = loadState.Complete
    self.first_check = true
    self.will_load_scene = false
    self.target_scene_id = nil
    self.loading_scene_id = nil
end

function LoadingCtrl:__delete()
    if self.view then
        self.view:DeleteMe()
        self.view = nil
    end
end

function LoadingCtrl:IsLoading()
    return self.state ~= loadState.Complete
end

function LoadingCtrl:OpenView(view_type)
    if self.view:IsOpen() then
        return
    end
end

