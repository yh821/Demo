require("logic/CtrlManager")
require("common/BindTool")
require("common/BaseView")

Game = Game or {}

function Game.Start()
    print_log("[Game]Start")

    PushCtrl(CtrlManager.New())

    MainUiCtrl.Instance:Open()

    Scene.Instance:CreateCamera()
    Scene.Instance:CreateMainRole()

    LoginCtrl.CreateClickEffectCanvas(MainUiCtrl.Instance)
end

function Game.OnDestroy()
    print_log("[Game]OnDestroy")
end

Game.Start()