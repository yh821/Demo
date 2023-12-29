---
--- Created by Hugo
--- DateTime: 2020/10/26 10:12
---

---@class SelectorNode : CompositeNode
SelectorNode = BaseClass(CompositeNode)

function SelectorNode:Tick(delta_time)
    local abort_type = self:GetAbortType()
    for i, v in ipairs(self._children) do
        if abort_type == eAbortType.Both or abort_type == eAbortType.Lower then
            if v:IsCondition() and v:IsNotExecuted() then
                self:SetNeedReevaluate()
            end
        end
        if abort_type == eAbortType.Both or abort_type == eAbortType.Self then
            if v:IsCondition() and v:IsExecuted() and self:IsRunning() then
                if v:SetState(v:Tick(delta_time)) and v:IsSucceed() then
                    self:StartAbortNode(i + 1)
                    return v:GetState()
                end
            end
        end
        if v:IsNotExecuted() or v:IsRunning() then
            v:SetState(v:Tick(delta_time))
            if v:GetState() ~= eNodeState.Failure then
                return v:GetState()
            end
        elseif v:IsComposite() and v:IsNeedReevaluate() then
            local state = v:ReevaluateNode(delta_time)
            if state == eNodeState.Success or state == eNodeState.Failure then
                self:StartAbortNode(i + 1)
            end
        end
    end
    return eNodeState.Success
end
