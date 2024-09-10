---
--- Created by Hugo
--- DateTime: 2024/2/27 15:35
---

---@class LoginCtrl : BaseClass
LoginCtrl = LoginCtrl or BaseClass()

function LoginCtrl:__init()
    if LoginCtrl.Instance then
        print_error("[LoginCtrl] attempt to create singleton twice!")
        return
    end
    LoginCtrl.Instance = self
end

function LoginCtrl:__delete()
    LoginCtrl.Instance = nil
end

function LoginCtrl.CreateClickEffectCanvas(root)
    --local ui_root = GameObject.Find("GameRoot/UiLayer").transform
    local ui_root = GameObject.Find("GameRoot").transform
    local bundle, asset = "uis/view/clickeffect_prefab", "ClickEffectCanvas"
    local loader = LoadUtil.AllocAsyncLoader(root, "ClickEffectCanvas")
    if ui_root then
        loader:SetParent(ui_root)
    end
    loader:Load(bundle, asset, function(obj)
        if IsNil(obj) then
            return
        end
        --local block = obj:GetComponent(typeof(UnityEngine.UI.Graphic))

    end)
end
