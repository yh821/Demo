﻿---
--- Created by Hugo
--- DateTime: 2023/4/26 21:18
---

require("game/mainui/MainUiData")
require("game/mainui/MainUiView")

---@class MainUiCtrl : BaseController
---@field view MainUiView
MainUiCtrl = MainUiCtrl or BaseClass(BaseController)

function MainUiCtrl:__init()
    if MainUiCtrl.Instance then
        print_error("[PromptCtrl] attempt to create singleton twice!")
        return
    end
    MainUiCtrl.Instance = self

    self.data = MainUiData.New()
    self.view = MainUiView.New()


end

function MainUiCtrl:__delete()
    self.gameObject = nil
    self.transform = nil
    self.panel = nil
    self.prompt = nil

    self.data:DeleteMe()
    self.data = nil

    self.view:DeleteMe()
    self.view = nil

    MainUiCtrl.Instance = nil
end

function MainUiCtrl:Open()
    --PanelMgr:CreatePanel("MainUi", BindTool.Bind(self.OnCreate, self))
    ViewManager.Instance:OpenView("MainUi", BindTool.Bind(self.LoadCallback, self))
end

function MainUiCtrl:LoadCallback(obj)
    self.gameObject = obj
    self.view:AddNodeList(obj)
    self.view:LoadCallback()
end