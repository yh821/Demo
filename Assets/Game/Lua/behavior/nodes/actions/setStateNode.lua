---
--- Created by Hugo
--- DateTime: 2020/12/07 16:52
---

---@class SetStateNode : ActionNode
SetStateNode = BaseClass(ActionNode)

function SetStateNode:Start()
    if self.data and self.data.stateId then
        self.owner:SetStateId(self.data.stateId)
        return eNodeState.Success
    else
        return eNodeState.Failure
    end
end

