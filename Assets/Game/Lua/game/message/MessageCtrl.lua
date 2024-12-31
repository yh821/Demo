require("game/message/MessageView")

--系统消息
---@class MessageCtrl : BaseController
MessageCtrl = MessageCtrl or BaseClass(BaseController)

function MessageCtrl:__init()
    if MessageCtrl.Instance then
        print_error("[MessageCtrl] attempt to create singleton twice!")
        return
    end
    MessageCtrl.Instance = self

    self.next_time = 0
    self.index = 1
    self.msg_list = {}
    self.label_list = {}

    Runner.Instance:AddRunObj(self, 3)
end

function MessageCtrl:__delete()
    Runner.Instance:RemoveRunObj(self)

    self.msg_list = nil
    self.next_time = nil
    for k, v in pairs(self.label_list) do
        ResPoolMgr.Instance:Release(v.view.gameObject)
        v:DeleteMe()
    end
    self.label_list = nil

    MessageCtrl.Instance = nil
end

local add_time = 0
local normal_time = 0.6
local time_int = normal_time
local normal_speed = 1.5
local show_speed = normal_speed
local show_count = 0
local max_speed = 18
local min_time = 0.05
local max_count = 20

function MessageCtrl:ShowMessage(msg, speed)
    print_log("提示消息:", msg)
    if #self.msg_list > max_count then
        table.remove(self.msg_list, 1)
    end
    table.insert(self.msg_list, { msg, speed })

    show_count = 0
    for k, v in pairs(sel.label_list) do
        if v:TipsVisible() then
            show_count = show_count + 1
        end
    end

    show_count = show_count + #self.msg_list
    time_int = show_count > 3 and normal_time / (show_count - 2) or normal_time
    time_int = math.max(time_int, min_time)
end

function MessageCtrl:RealShowMessage(msg, speed)
    speed = speed or 1
    local label = self.label_list[self.index]
    if label then
        label:CloseTips()
        label:Show(msg, speed, 0)
    else
        local canvas_transform = MessageCtrl.GetMessageCanvas().transform
        local obj = ResPoolMgr.Instance:TryGetGameObject("uis/view/load_prefab", "MessageView", canvas_transform)
        label = MessageView.New(obj)
        self.label_list[self.index] = label
        label:Show(msg, speed, 0)
    end

    for k, v in pairs(sel.label_list) do
        if k ~= self.index then
            v:AddIndex()
        end
    end

    self.index = self.index + 1
    if self.index > 3 then
        self.index = 1
    end
end

function MessageCtrl:Update(now_time, delta_time)
    if self.msg_list[1] and (now_time - add_time > time_int) then
        add_time = now_time
        show_count = 0
        for k, v in pairs(self.label_list) do
            if v:TipsVisible() then
                show_count = show_count + 1
            end
        end
        show_count = show_count + #self.msg_list
        show_speed = show_count > 3 and normal_speed * (show_count - 2) or normal_speed
        show_speed = math.min(show_speed, max_speed)
        time_int = show_count > 3 and normal_time / (show_count - 2) or normal_time
        time_int = math.max(time_int, min_time)
        local msg = table.remove(self.msg_list, 1)
        self:RealShowMessage(msg[1], show_speed)
        for k, v in pairs(self.label_list) do
            v:changeSpeed(show_speed)
        end
    end
end

function MessageCtrl.GetMessageCanvas()
    if not MessageCtrl.floating_canvas then
        local obj = ResMgr.Instance:Instantiate(BaseView.GetViewTemplate())
        obj.name = "MessageCanvas"
        obj:SetActive(true)

        local canvas = obj:GetComponent(typeof(UnityEngine.Canvas))
        canvas.overrideSorting = true
        canvas.sortingOrder = ViewLayer.PopHigh * 1000 + 900
        canvas.worldCamera = UICamera

        obj:GetComponent(typeof(UnityEngine.UI.GraphicRaycaster)).enabled = false

        local canvas_transform = canvas.transform
        canvas_transform:SetParent(UiLayer.transform, false)
        canvas_transform:SetLocalScale(1, 1, 1)

        local rect = canvas_transform:GetComponent(typeof(UnityEngine.RectTransform))
        rect.anchorMin = Vector2(0, 0)
        rect.anchorMax = Vector2(1, 1)
        rect.sizeDelta = Vector2(0, 0)

        MessageCtrl.floating_canvas = canvas
    end
    return MessageCtrl.floating_canvas
end