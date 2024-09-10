---
--- Created by Hugo
--- DateTime: 2023/4/27 15:55
---

local TypeUnityPrefab = typeof(UnityEngine.GameObject)
local TypeUnityTexture = typeof(UnityEngine.Texture)
local TypeUnitySprite = typeof(UnityEngine.Sprite)
local TypeTextAsset = typeof(UnityEngine.TextAsset)
local TypeUnityMaterial = typeof(UnityEngine.Material)
local TypeUnityAudio = typeof(UnityEngine.Audio.AudioMixer)
local TypeRuntimeAnimatorController = typeof(UnityEngine.RuntimeAnimatorController)

local POOL_TYPE_TBL = {
    [TypeUnityPrefab] = ResPoolMgr.GetPrefab,
    [TypeUnityTexture] = ResPoolMgr.GetTexture,
    [TypeUnitySprite] = ResPoolMgr.GetSprite,
    [TypeTextAsset] = ResPoolMgr.GetTextAsset,
    [TypeRuntimeAnimatorController] = ResPoolMgr.GetAnimatorController,
    [TypeUnityAudio] = ResPoolMgr.GetAudio,
    [TypeUnityMaterial] = ResPoolMgr.GetMaterial,
}

---@class ResLoader : BaseClass
ResLoader = BaseClass()

function ResLoader:__init()
    self.is_deleted = false
    self.cur_tbl = nil
    self.wait_tbl = nil
    self.is_async = true
    self.load_priority = ResLoadPriority.high
    self.is_loading = false
end

function ResLoader:__delete()
    if not self.__is_had_del_in_cache then
        self.__is_had_del_in_cache = true
        if self.__loader_key and self.__loader_owner and self.__loader_owner.__res_loaders then
            self.__loader_owner.__res_loaders[self.__loader_key] = nil
        end
    end

    self:Destroy()
    self.is_deleted = true

    if self.wait_tbl then
        CbdPool.ReleaseCbData(self.wait_tbl)
        self.wait_tbl = nil
    end
end

function ResLoader:Destroy()
    if self.cur_tbl then
        if self.cur_tbl[CbdIndex.prefab] then
            ResPoolMgr.Instance:Release(self.cur_tbl[CbdIndex.prefab])
        end
        CbdPool.ReleaseCbData(self.cur_tbl)
        self.cur_tbl = nil
    end
end

function ResLoader:SetIsAsyncLoad(is_async)
    self.is_async = is_async
end

function ResLoader:SetLoadPriority(load_priority)
    if load_priority then
        self.load_priority = load_priority
    end
end

function ResLoader:Load(bundle_name, asset_name, asset_type, load_callback, cb_data)
    asset_type = asset_type or TypeUnityPrefab

    if IsNilOrEmpty(bundle_name) or IsNilOrEmpty(asset_name) then
        return
    end

    if POOL_TYPE_TBL[asset_type] == nil then
        print_error("[ResLoader] load fail, not support asset_type:", bundle_name, asset_name)
        return
    end

    --如果跟上次加载的资源相同则不再进行请求加载，若资源已经存在则直接回调
    if self.cur_tbl
            and self.cur_tbl[CbdIndex.bundle] == bundle_name
            and self.cur_tbl[CbdIndex.asset] == asset_name
            and self.cur_tbl[CbdIndex.type] == asset_type
    then
        if load_callback then
            load_callback(self.cur_tbl[CbdIndex.prefab], cb_data)
        end
        return
    end

    if UNITY_EDITOR then
        if not EditorResourceMgr.IsExistsAsset(bundle_name, asset_name) then
            print_error("加载资源不存在，马上检查：", bundle_name, asset_name)
            return
        end
    end

    --如果正在加载则等待
    if self.is_loading then
        self.wait_tbl = CbdPool.CreateCbData()
        self.wait_tbl[CbdIndex.bundle] = bundle_name
        self.wait_tbl[CbdIndex.asset] = asset_name
        self.wait_tbl[CbdIndex.type] = asset_type
        self.wait_tbl[CbdIndex.callback] = load_callback
        self.wait_tbl[CbdIndex.cb_data] = cb_data
    else
        self:Destroy()
        self:DoLoad(bundle_name, asset_name, asset_type, load_callback, cb_data)
    end
end

function ResLoader:DoLoad(bundle_name, asset_name, asset_type, load_callback, cb_data)
    local cbd = CbdPool.CreateCbData()
    cbd[CbdIndex.self] = self
    cbd[CbdIndex.bundle] = bundle_name
    cbd[CbdIndex.asset] = asset_name
    cbd[CbdIndex.type] = asset_type
    cbd[CbdIndex.callback] = load_callback
    cbd[CbdIndex.cb_data] = cb_data

    self.is_loading = true

    POOL_TYPE_TBL[asset_type](
            ResPoolMgr.Instance,
            bundle_name,
            asset_name,
            ResLoader.OnLoadComplete,
            self.is_async,
            cbd,
            self.load_priority)
end

function ResLoader.OnLoadComplete(res, cb_data)
    local self = cb_data[CbdIndex.self]
    local bundle_name = cb_data[CbdIndex.bundle]
    local asset_name = cb_data[CbdIndex.asset]
    local asset_type = cb_data[CbdIndex.type]
    local load_callback = cb_data[CbdIndex.callback]
    local cbd = cb_data[CbdIndex.cb_data]
    CbdPool.ReleaseCbData(cb_data)

    self.is_loading = false

    --如果加载器已被释放则释放当前加载完成的
    if self.is_deleted then
        if res then
            ResPoolMgr.Instance:Release(res)
        end
        return
    end

    --如果有等待加载的资源则释放当前加载的
    if self.wait_tbl then
        if res then
            ResPoolMgr.Instance:Release(res)
        end
        local t = self.wait_tbl
        self.wait_tbl = nil
        self:DoLoad(
                t[CbdIndex.bundle],
                t[CbdIndex.asset],
                t[CbdIndex.type],
                t[CbdIndex.callback],
                t[CbdIndex.cb_data])
        CbdPool.ReleaseCbData(t)
        return
    end

    if self.cur_tbl then
        print_error("[ResLoader] OnLoadComplete data unexpected exit:", bundle_name, asset_name)
    end

    self.cur_tbl = CbdPool.CreateCbData()
    self.cur_tbl[CbdIndex.bundle] = bundle_name
    self.cur_tbl[CbdIndex.asset] = asset_name
    self.cur_tbl[CbdIndex.type] = asset_type
    self.cur_tbl[CbdIndex.prefab] = res

    if load_callback then
        load_callback(res, cbd)
    end
end