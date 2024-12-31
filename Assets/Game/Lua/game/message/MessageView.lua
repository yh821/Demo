---
--- Created by Hugo
--- DateTime: 2024/10/8 21:42
---

---@class MessageView : CellWidget
MessageView = MessageView or BaseClass(CellWidget)

function MessageView:__init()
    self.message = ""
    self.anim_speed = 1
    self.converiton = -1
    self.index = 0

    self.message_text = self.node_list["MessageText"]

    self.anim = self.message_view.animator
end

function MessageView:__delete()
end