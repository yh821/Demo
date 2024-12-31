---
--- Created by Hugo
--- DateTime: 2024/10/7 18:17
---

---@class ConfigManager : BaseClass
ConfigManager = ConfigManager or BaseClass()

function ConfigManager:__init()
    if ConfigManager.Instance then
        print_error("attempt to create singleton twice!")
        return
    end
    ConfigManager.Instance = self
end

function ConfigManager:__delete()
    ConfigManager.Instance = nil
end

function ConfigManager:GetConfig(file_name)

end

--临时
local first_scene_cfg = {
    id = 10001,
    scene_type = 1,
    bundle_name = "bundle",
    asset_name = "asset",
}
function ConfigManager:GetSceneConfig(scene_id)
    return first_scene_cfg
end