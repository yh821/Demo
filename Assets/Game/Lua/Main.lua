GameObject = UnityEngine.GameObject

local UnityApplication = UnityEngine.Application

UNITY_EDITOR = false --编辑环境
INF = 9000000000000000 -- 1/0 --无穷大
IND = -9000000000000000 -- 0/0 --无穷小

IsLowMemorySystem = UnityEngine.SystemInfo.systemMemorySize <= 1500

mime = require("mime")
cjson = require("cjson.safe")

require("common/Util")
require("common/BaseClass")
require("common/U3DObject")
require("common/Vector3Pool")
require("common/SortTools")
require("common/CbdPool")
require("loader/ResUtil")
require("loader/LoadUtil")

function __TRACK_BACK__(msg)
    local track_text = debug.traceback(tostring(msg))
    print_error(track_text, "LUA ERROR")
    return false
end

function TryCall(func, p1, p2, p3)
    return xpcall(func, __TRACK_BACK__, p1, p2, p3)
end

local ctrl_list = {}
function PushCtrl(ctrl)
    ctrl_list[ctrl] = ctrl
end
function PopCtrl(ctrl)
    ctrl_list[ctrl] = nil
end

--主入口函数。从这里开始lua逻辑
function Main()
    print("[Main] logic start")

    ResUtil.SetBaseCachePath(string.format("%s/%s", UnityApplication.persistentDataPath, "BundleCache"))

    if GAME_ASSET_BUNDLE then
        ResUtil.InitEncryptKey()
        if ResUtil.is_ios_encrypt_asset then
            ResUtil.SetBaseCachePath(string.format("%s/%s", UnityApplication.persistentDataPath, EncryptMgr.GetEncryptPath("BundleCache")))
        end
        require("loader/BundleLoader")
    else
        require("loader/SimulationLoader")
    end

    require("loader/GameObjAttachEventHandle")
    require("loader/LoadRawImageEventHandle")
    require("loader/EffectEventHandle")
end

local UnityTime = UnityEngine.Time
function GameUpdate()
    local time = UnityTime.unscaledTime
    local delta_time = UnityTime.unscaledDeltaTime

    for i, v in pairs(ctrl_list) do
        v:Update(time, delta_time)
    end

    if Vector3Pool then
        Vector3Pool.Update(time, delta_time)
    end
end

function GameLateUpdate()
end

function GameFocus(focus)
    if EventSystem and EventSystem.Instance then
        EventSystem.Instance:Fire(SystemEventType.GAME_FOCUS, focus)
    end
end

function GamePause(pause)
    if EventSystem and EventSystem.Instance then
        EventSystem.Instance:Fire(SystemEventType.GAME_PAUSE, pause)
    end
end

function EnabledGameObjAttachEvent(list)
    GameObjAttachEventHandle.EnabledGameObjAttachEvent(list)
end

function DisabledGameObjAttachEvent(list)
    GameObjAttachEventHandle.DisabledGameObjAttachEvent(list)
end

function DestroyGameObjAttachEvent(list)
    GameObjAttachEventHandle.DestroyGameObjAttachEvent(list)
end

function EnabledLoadRawImageEvent(list)
    LoadRawImageEventHandle.EnabledLoadRawImageEvent(list)
end

function DisabledLoadRawImageEvent(list)
    LoadRawImageEventHandle.DisabledLoadRawImageEvent(list)
end

function DestroyLoadRawImageEvent(list)
    LoadRawImageEventHandle.DestroyLoadRawImageEvent(list)
end

function ProjectileSingleEffectEvent(hit_effect, position, rotation, hit_effect_with_rotation, source_scale)
    EffectEventHandle.ProjectileSingleEffectEvent(hit_effect, position, rotation, hit_effect_with_rotation, source_scale)
end

function UIMouseClickEffectEvent(effects, canvas, mouse_click_transform)
    EffectEventHandle.UIMouseClickEffectEvent(effects, canvas, mouse_click_transform)
    if EventSystem.Instance then
        EventSystem.Instance:Fire(TouchEventType.UI_CLICK)
    end
end

function OnApplicationQuit()
end

Main()