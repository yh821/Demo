local LUA_ASSET_BUNDLE_PREFIX
local LUA_ASSET_PREFIX

if UNITY_IOS or UNITY_STANDALONE then
    LUA_ASSET_BUNDLE_PREFIX = 'lua/'
    LUA_ASSET_PREFIX = 'Assets/Game/LuaBundle/'
else
    if GameRoot.IsAndroid64() then
        LUA_ASSET_BUNDLE_PREFIX = 'luajit64/'
        LUA_ASSET_PREFIX = 'Assets/Game/LuaBundleJit64/'
    else
        LUA_ASSET_BUNDLE_PREFIX = 'luajit32/'
        LUA_ASSET_PREFIX = 'Assets/Game/LuaBundleJit32/'
    end
end

local SysFile = System.IO.File
local UnityApplication = UnityEngine.Application
local UnityAppStreamingAssetsPath = UnityApplication.streamingAssetsPath
local UnityAppPersistentDataPath = UnityApplication.persistentDataPath

if not UNITY_EDITOR then
    local cacheDir = EncryptMgr.GetEncryptPath('BundleCache')
    local cachePath = string.format('%s/%s', UnityAppPersistentDataPath, cacheDir)

    local luaAssetBundle = 'LuaAssetBundle/LuaAssetBundle.lua'
    local cacheLuaAssetBundle = EncryptMgr.GetEncryptPath(luaAssetBundle)
    local luaAssetBundleData

    local cacheFullPath = string.format('%s/%s', cachePath, cacheLuaAssetBundle)
    if SysFile.Exists(cacheFullPath) then
        luaAssetBundleData = SysFile.ReadAllText(cacheFullPath)
    else
        local aliasPath = GameRoot.GetAliasResPath('AssetBundle/' .. luaAssetBundle)
        if EncryptMgr.IsEncryptAsset then
            luaAssetBundleData = EncryptMgr.ReadEncryptFile(string.format('%s/%s', UnityAppStreamingAssetsPath, aliasPath))
        else
            luaAssetBundleData = StreamingAssets.ReadAllText(aliasPath)
        end
    end

    local pattern = LUA_ASSET_BUNDLE_PREFIX .. '.+'
    local luaBundleInfos = loadstring(luaAssetBundleData)().bundleInfos

    for bundleName, bundleInfo in pairs(luaBundleInfos) do
        if string.match(bundleName, pattern) then
            local hash = bundleInfo.hash
            local relativePath = string.format('LuaAssetBundle/%s-%s', bundleName, hash)
            relativePath = EncryptMgr.GetEncryptPath(relativePath)
            local path = string.format('%s/%s', cachePath, relativePath)
            if not SysFile.Exists(path) then
                path = string.format('%s/AssetBundle/', UnityAppStreamingAssetsPath, relativePath)
            end
            for i, file in ipairs(bundleInfo.deps) do
                AddLuaBundle(file, path)
            end
        end
    end
end

AddLuaBundle = nil