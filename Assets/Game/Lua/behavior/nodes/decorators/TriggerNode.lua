---
--- Created by Hugo
--- DateTime: 2024/1/2 21:52
---

---@class TriggerNode : DecoratorNode
TriggerNode = TriggerNode or BaseClass(DecoratorNode)

function TriggerNode:Tick(delta_time)
    if self.type == nil then
        self.type = self.data and self.data.triggerType or BtTriggerType.Equals
        self.value = self.data and self.data.triggerValue or 0
    end
    local condition = false
    local value = self.owner:GetTrigger()
    if self.type == BtTriggerType.Equals then
        condition = self.value == value
    elseif self.type == BtTriggerType.NotEquals then
        condition = self.value ~= value
    elseif self.type == BtTriggerType.Greater then
        condition = self.value < value
    elseif self.type == BtTriggerType.Less then
        condition = self.value > value
    end
    if condition then
        local v = self._children[1]
        if v:IsNotExecuted() or v:IsRunning() then
            v:SetState(v:Tick(delta_time))
            return v:GetState()
        end
    end
    return eNodeState.Failure
end