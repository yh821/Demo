---
--- Created by Hugo
--- DateTime: 2023/5/6 0:18
---

local TypeUINameTable = typeof(UINameTable)

ViewLayer = {
    SceneText = 0, --场景UI
    FloatText = 1, --飘字
    MainUILow = 2, --主界面(低)
    MainUI = 3, --主界面
    MainUIHigh = 4, --主界面(高)
    Normal = 5, --普通界面
    PopLow = 6, --弹窗(低)
    Pop = 7, --弹窗
    PopHigh = 8, --弹窗(高)
    Guide = 9, --指引
    Loading = 10, --加载界面
    LoadingHigh = 11, --加载界面弹窗
    Disconnect = 12, --断线重连
    Standby = 13, --待机遮罩
    MaxLayer = 14
}

ViewType = {
    Normal = 1, --不关闭主相机
    FullScreen = 2, --关闭主相机
    UseDepth = 3, --模糊截图后, 关闭主相机
    UseShot = 4, --直接截图后, 关闭主相机
}

MaskAlpha = {
    Half = 128 / 255,
    Normal = 185 / 255,
    Tips = 200 / 255,
    Zero = 0,
    One = 1,
}

--local ReferenceResolution
local BaseViewTemplate = GameObject.Find("GameRoot/BaseView")
if BaseViewTemplate then
    BaseViewTemplate:SetActive(false)
    --ReferenceResolution =
end

UICameraGameObject = GameObject.Find("GameRoot/UiLayer/UICamera")
UICamera = UICameraGameObject:GetComponent(typeof(UnityEngine.Camera))
--SnapShot = UICameraGameObject:GetComponent(typeof(UICameraSnapShot))

---@class BaseView : BaseClass
BaseView = BaseView or BaseClass()

function BaseView:__init(view_name)
    if IsNilOrEmpty(view_name) then
        view_name = GetClassName(self)
    end

    self.view_name = view_name
    self.view_layer = ViewLayer.Normal
    self.view_type = ViewType.Normal
    self.mask_alpha = MaskAlpha.Normal
    self.show_index = -1
    self.default_index = 0
    self.is_async_load = true
    self.is_open = false
    self.is_loaded = false
    self.is_active_close = true
    self.is_mask_click = true

    ViewManager.Instance:AddView(self, view_name)
end

function BaseView:__delete()
    ViewManager.Instance:RemoveView(self.view_name)
    self:Release()
end

function BaseView.GetViewTemplate()
    return BaseViewTemplate
end

function BaseView:AddNodeList(gameObject)
    local name_table = gameObject:GetComponent(TypeUINameTable)
    self.node_list = U3DNodeList(name_table, self)
end

function BaseView:IsOpen()

end

function BaseView:Close()

end

function BaseView:Release()

end

function BaseView:CanActiveClose()
    return self.is_active_close
end

function BaseView:SetViewLayer(view_layer)
    self.view_layer = view_layer or ViewLayer.Normal
end

function BaseView:SetViewType(view_type)
    self.view_type = view_type or ViewType.Normal
end

function BaseView:SetMaskAlpha(value)
    self.mask_alpha = value or MaskAlpha.Normal
end

--打开调用(在加载之前调用)
function BaseView:OpenCallBack(index)
    --override
end

--打开标签调用(在加载之后调用)
function BaseView:OpenIndexCallBack(index)
    --override
end

--切换标签调用
function BaseView:ShowIndexCallBack(index)
    --override
end

--关闭标签调用
function BaseView:CloseIndexCallBack(index)
    --override
end

--加载后调用
function BaseView:LoadCallBack()
    --override
end

--加载后调用
function BaseView:LoadIndexCallBack(index)
    --override
end

--关闭前调用
function BaseView:CloseCallBack()
    --override
end

--销毁前调用
function BaseView:ReleaseCallBack()
    --override
end

function BaseView:OnFlush(params, index)
    --override
end
