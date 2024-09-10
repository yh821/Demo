---
--- Created by Hugo
--- DateTime: 2024/2/18 20:39
---

---@class EffectEventHandle
EffectEventHandle = {}

local index = 0
function EffectEventHandle.UIMouseClickEffectEvent(effects, canvas, mouse_click_transform)
    if effects.Length > 0 then
        local obj = effects[index]
        index = index + 1
        index = index % effects.Length
        if obj ~= nil then
            local effect = ResPoolMgr.Instance:TryGetGameObjectInPrefab(obj)
            if IsNil(effect) then
                return
            end

            local rect = effect.transform
            rect:SetParent(mouse_click_transform, false)
            rect.localScale = Vector3.one

            local _, local_pos_tbl = UnityEngine.RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform, UnityEngine.Input.mousePosition, canvas.worldCamera, Vector2(0, 0))
            rect.localPosition = Vector3(local_pos_tbl.x, local_pos_tbl.y, 0)
            effect:GetComponent(typeof(UnityEngine.Animator)):WaitEvent("exit", function()
                if effect then
                    ResPoolMgr.Instance:Release(effect)
                end
                effect = nil
            end)
        end
    end
end

function EffectEventHandle.ProjectileSingleEffectEvent(hit_effect, position, rotation, hit_effect_with_rotation, source_scale)
    local go = hit_effect.gameObject
    local obj = ResPoolMgr.Instance:TryGetGameObjectInPrefab(go)
    if not obj then
        return
    end

    local effect = obj:GetComponent(typeof(EffectController))
    if effect == nil then
        ResPoolMgr.Instance:Release(obj)
        return
    end

    if hit_effect_with_rotation then
        effect.transform:SetPositionAndRotation(position, rotation)
    else
        effect.transform.position = position
    end
    effect.transform.localScale = source_scale

    effect:Reset()
    effect:WaitFinish(function()
        ResPoolMgr.Instance:Release(obj)
    end)
    effect:Play()
end