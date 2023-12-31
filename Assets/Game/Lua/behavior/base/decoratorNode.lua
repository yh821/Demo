---
--- Created by Hugo
--- DateTime: 2020/10/26 11:22
---

---@class DecoratorNode : ParentNode
DecoratorNode = BaseClass(ParentNode)

---@param node TaskNode
function DecoratorNode:AddChild(node)
    if self._children == nil then
        self._children = {}
    end
    if #self._children > 0 then
        table.remove(self._children, 1)
    end
    table.insert(self._children, node)
end

function DecoratorNode:IsDecorator()
    return true
end
