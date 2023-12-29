---
--- Created by Hugo
--- DateTime: 2022/5/6 0:18
---

---@class MainUiView : BaseView
MainUiView = MainUiView or BaseClass(BaseView)

--启动事件--
function MainUiView:__init(view_name)
    self.view_name = view_name
end

function MainUiView:__delete()
end

function MainUiView:LoadCallback()
    self.node_list["btn_add_pet"].button:AddClickListener(BindTool.Bind(self.OnAddPet, self))
    self.node_list["btn_open_ai"].button:AddClickListener(BindTool.Bind(self.OnOpenAi, self))
    self.node_list["btn_clean_all"].button:AddClickListener(BindTool.Bind(self.OnCleanSceneObj, self))
    for i = 1, 4 do
        self.node_list["btn_add_monster" .. i].button:AddClickListener(BindTool.Bind(self.OnAddMonster, self, i))
    end

    self:JoystickLoadCallback()
end

function MainUiView:OnOpenAi()
    print_log("切换AI开关")
    AiManager.Instance:SwitchTick()
end

function MainUiView:OnAddMonster(index)
    local role = Scene.Instance:GetMainRole()
    local pos = role:GetDrawObj():GetPosition() or Vector3Pool.GetTemp(0, 0, 0)
    local monster = Scene.Instance:CreateMonster({ pos_x = pos.x, pos_y = pos.y, pos_z = pos.z, name = "Zombie" })
    local bt = AiManager.Instance:BindBT(monster, "monster" .. index)
    bt:SetSharedVar(BtConfig.target_obj_key, role)
end

function MainUiView:OnAddPet()
    local role = Scene.Instance:GetMainRole()
    local pos = role:GetDrawObj():GetPosition() or Vector3Pool.GetTemp(0, 0, 0)
    local pet = Scene.Instance:CreatePet({ pos_x = pos.x, pos_y = pos.y, pos_z = pos.z, name = "Pig" })
    local bt = AiManager.Instance:BindBT(pet, "pet")
    bt:SetSharedVar(BtConfig.target_obj_key, role)
end

function MainUiView:OnCleanSceneObj()
    print_log("删除除主角外的场景对象")
    Scene.Instance:DeleteObjListByType(SceneObjType.Monster)
    Scene.Instance:DeleteObjListByType(SceneObjType.Pet)
end

--关闭事件--
function MainUiView:Close()
    PanelManager:ClosePanel(CtrlNames.Message);
end

--------------------------------------------------Joystick Begin--------------------------------------------------------

local DISTANCE_TO_MOVE = 30 * 30
local DISTANCE_TO_END = 25 * 25

function MainUiView:JoystickLoadCallback()
    self.is_touched = false
    self.is_joystick = false
    self.joystick_finger_index = -1
    self.control_id = -1

    local joystick = self.node_list["Joystick"].joystick
    joystick:AddTouchedListener(BindTool.Bind(self.OnTouched, self))
    joystick:AddDragUpdateListener(BindTool.Bind(self.OnDragUpdate, self))
    joystick:AddDragEndListener(BindTool.Bind(self.OnDragEnd, self))

    self.swipe_start_handle = BindTool.Bind1(self.OnSwipeStart, self)
    EasyTouch.On_SwipeStart = EasyTouch.On_SwipeStart + self.swipe_start_handle
    self.swipe_handle = BindTool.Bind1(self.OnSwipe, self)
    EasyTouch.On_Swipe = EasyTouch.On_Swipe + self.swipe_handle
    self.swipe_end_handle = BindTool.Bind1(self.OnSwipeEnd, self)
    EasyTouch.On_SwipeEnd = EasyTouch.On_SwipeEnd + self.swipe_end_handle
    self.pinch_handle = BindTool.Bind1(self.OnPinch, self)
    EasyTouch.On_Pinch = EasyTouch.On_Pinch + self.pinch_handle

    self.update_timer = TimerQuest.Instance:AddRunQuest(BindTool.Bind1(self.JoystickUpdate, self), 0)
end

function MainUiView:OnTouched(is_touched, finger_index)
    self.joystick_finger_index = finger_index
    self.is_touched = is_touched
    if not is_touched then
        self.is_joystick = false
    end
end

function MainUiView:OnDragUpdate(x, y)
    local delta = x * x + y * y
    local is_move
    if delta >= DISTANCE_TO_MOVE then
        is_move = true
    elseif delta < DISTANCE_TO_END then
        is_move = false
    else
        is_move = self.control_id == 2
    end
    if is_move then
        if not self.control_id then
            self:__OnControllerBegin(0)
        end
        self:__OnControllerUpdate(0, x, y)
    else
        self:__OnControllerEnd(0)
    end
    EventSystem.Instance:Fire(TouchEventType.JOYSTICK_UPDATE)
end

function MainUiView:OnDragEnd(x, y)
    self:__OnControllerEnd(0, true)
    EventSystem.Instance:Fire(TouchEventType.JOYSTICK_END)
end

function MainUiView:IsJoystick()
    return self.is_joystick
end

function MainUiView:__OnControllerBegin(id)
    if self.control_id > 0 and self.control_id ~= id then
        return
    end
    local main_role = Scene.Instance:GetMainRole()
    if not main_role:CanDoMove() then
        return
    end
    self.control_id = id
    EventSystem.Instance:Fire(TouchEventType.TOUCH_BEGIN)
end

function MainUiView:__OnControllerUpdate(id, x, y)
    if self.control_id ~= id then
        return
    end
    local main_role = Scene.Instance:GetMainRole()
    if not main_role:CanDoMove() then
        self.control_id = -1
        return
    end
end

function MainUiView:__OnControllerEnd(id)
    if self.control_id ~= id then
        return
    end
    self.control_id = -1
    local main_role = Scene.Instance:GetMainRole()
    if not main_role:CanDoMove() then
        self.control_id = -1
        return
    end
    EventSystem.Instance:Fire(TouchEventType.TOUCH_END)
end

function MainUiView:OnSwipeStart(gesture)
    local finger_index = gesture.fingerIndex
    if finger_index ~= self.joystick_finger_index and not self.is_drag then
        self.is_drag = true
        self.swipe_finger_index = finger_index
        --EventSystem.Instance:Fire()
    end
end

function MainUiView:OnSwipe(gesture)
    if not self.is_drag or (self.is_touched and gesture.fingerIndex == self.joystick_finger_index) then
        return
    end
    local x = gesture.swipeVector.x
    local y = gesture.swipeVector.y
    x = MathClamp(x, -20, 20)
    if not IsNil(MainCamera) then
        MainCamera:Swipe(x, y)
    end
    --EventSystem.Instance:Fire()
end

function MainUiView:OnSwipeEnd(gesture)
    if gesture.fingerIndex ~= self.joystick_finger_index then
        return
    end
    self.is_drag = false
    self.swipe_finger_index = -1
    --EventSystem.Instance:Fire()
end

function MainUiView:OnPinch(gesture)
    if not self.is_touched and not IsNil(MainCamera) then
        local pinch = MathClamp(gesture.deltaPinch, -33, 33)
        MainCamera:Pinch(pinch)
    end
end

function MainUiView:JoystickUpdate()

end

---------------------------------------------------Joystick End---------------------------------------------------------