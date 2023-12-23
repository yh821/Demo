---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by yuanhuan.
--- DateTime: 2020/12/07 16:54
---

---@class speakNode : ActionNode
speakNode = BaseClass(ActionNode)

function speakNode:Start()
    if self.hudId == nil then
        self.hudId = hudControl:addHUD(self.owner.guid)
    end
    local widget = hudControl:getHUDWidget(self.hudId)
    if widget and self.data and self.data.say then
        widget:setText(self.data.say)
        return eNodeState.Success
    else
        return eNodeState.Failure
    end
end

function speakNode:Reset()
    self:shutUp()
end

function speakNode:Abort()
    self:shutUp()
end

function speakNode:shutUp()
    if self.hudId then
        hudControl:removeHUD(self.hudId)
        self.hudId = nil
    end
end