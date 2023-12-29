---
--- Created by Hugo
--- DateTime: 2022/1/20 15:16
---

---@class CompositeNode : ParentNode
CompositeNode = BaseClass(ParentNode)

eAbortType = {
    None = 0,
    Self = 1,
    Lower = 2,
    Both = 3
}

---@param node TaskNode
function CompositeNode:AddChild(node)
    if self._children == nil then
        self._children = {}
    end
    table.insert(self._children, node)
end

function CompositeNode:Clear()
    CompositeNode.super.Clear(self)
    self._abort_type = nil
    self._need_reevaluate = nil
end

function CompositeNode:IsComposite()
    return true
end

function CompositeNode:GetAbortType()
    if self._abort_type == nil then
        local abort = self.data and self.data.abort
        self._abort_type = eAbortType[abort] or eAbortType.None
    end
    return self._abort_type
end

---@type fun(parent:TaskNode)
local __AbortNode
__AbortNode = function(node)
    local children = node:GetChildren()
    if children then
        for _, v in ipairs(children) do
            __AbortNode(v)
        end
    else
        if node:IsRunning() then
            node:SetState(node:Abort())
        end
    end
end

function CompositeNode:StartAbortNode(start_index)
    for i = start_index, #self._children do
        __AbortNode(self._children[i])
    end
end

function CompositeNode:SetNeedReevaluate()
    self._need_reevaluate = true
end

function CompositeNode:IsNeedReevaluate()
    return self._need_reevaluate
end

function CompositeNode:ReevaluateNode(delta_time)
    for i, v in ipairs(self._children) do
        if v:IsSucceed() or v:IsFailed() then
            if v:IsCondition() then
                if v:SetState(v:Tick(delta_time)) then
                    return v:GetState()
                end
            elseif v:IsComposite() and v:IsNeedReevaluate() then
                local abort_type = v:GetAbortType()
                if abort_type == eAbortType.Both or abort_type == eAbortType.Lower then
                    return v:ReevaluateNode(delta_time)
                end
            end
        end
    end
end