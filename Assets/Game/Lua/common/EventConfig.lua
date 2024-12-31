---
--- Created by Hugo
--- DateTime: 2023/12/21 15:22
---

SceneEventType = {
    SCENE_START_LOAD = "scene_start_load", --开始加载场景
    SCENE_LOADING = "scene_loading", --正在加载场景
    SCENE_LOAD_COMPLETE = "scene_load_complete", --完成加载场景
}

ObjectEventType = {
    CREATE_OBJ = "create_obj",
    DELETE_OBJ = "delete_obj",

    CREATE_MAIN_ROLE = "create_main_role",
}

TouchEventType = {
    TOUCH_BEGIN = "touch_begin",
    TOUCH_MOVE = "touch_move",
    TOUCH_END = "touch_end",
    TOUCH_CANCEL = "touch_cancel",

    MOUSE_BUTTON_DOWN = "mouse_button_down",
    MOUSE_BUTTON_UP = "mouse_button_up",

    JOYSTICK_UPDATE = "joystick_update",
    JOYSTICK_END = "joystick_end",

    UI_CLICK = "ui_click",
}

SystemEventType = {
    GAME_FOCUS = "game_focus",
    GAME_PAUSE = "game_pause",
}
