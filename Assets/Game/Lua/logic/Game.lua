require("logic/CtrlManager")
require("common/BindTool")
require("game/widget/BaseView")

Game = Game or {}

function Game.Start()
    print_log("[Game]Start")

    PushCtrl(CtrlManager.New())

    MainUiCtrl.Instance:Open()

    local scene_id = 10001
    --EventSystem.Instance:Fire(SceneEventType.SCENE_START_LOAD, scene_id)
    Scene.Instance:CreateCamera()
    Scene.Instance:CreateMainRole()

    LoginCtrl.CreateClickEffectCanvas(MainUiCtrl.Instance)
end

function Game.OnDestroy()
    print_log("[Game]OnDestroy")
end

Game.Start()