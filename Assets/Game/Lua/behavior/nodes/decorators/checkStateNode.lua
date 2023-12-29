---
--- Created by Hugo
--- DateTime: 2020/11/30 21:31
---

---@class CheckStateNode : DecoratorNode
CheckStateNode = BaseClass(DecoratorNode)

function CheckStateNode:Update(delta_time)
    local stateId = self.owner:GetStateId()
    if stateId == self.stateId then
        if self._children then
            local v = self._children[1]
            if v:IsNotExecuted() or v:IsRunning() then
                v:SetState(v:Tick(delta_time))
                return v:GetState()
            end
        end
    end
    return eNodeState.Failure
end