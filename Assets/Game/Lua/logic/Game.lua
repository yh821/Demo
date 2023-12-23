require("logic/CtrlManager")
require("common/functions")
require("common/BindTool")

Game = Game or {}

function Game.Start()
    print_log("[Game]Start")
    AppConst.SocketPort = 2012
    AppConst.SocketAddress = "127.0.0.1"
    --networkMgr:SendConnect()

    require("game/Common/BaseView")

    PushCtrl(CtrlManager.New())

    MainUiCtrl.Instance:Open()

    Scene.Instance:CreateCamera()
    Scene.Instance:CreateMainRole()
end

function Game.OnDestroy()
    print_log("[Game]OnDestroy")
end