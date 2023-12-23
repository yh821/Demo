﻿---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by admin.
--- DateTime: 2023/4/26 14:06
---

local TypeMovableObject = typeof(MovableObject)

SceneObjType = {
    Unknown = 0,
    Role = 1,
    Monster = 2,
    Pet = 3,

    MainRole = 10,
}

---@class DrawObj : BaseClass
---@field part_list DrawPart[]
DrawObj = DrawObj or BaseClass()

DrawObj.AnimParamType = {
    ResetTrigger = 0,
    Trigger = 1,
    Boolean = 2,
    Float = 3,
    Integer = 4,
}

local function InitDrawObjPool()
    DrawObj.obj_list = {}
    DrawObj.obj_count = 0
    DrawObj.v_root = ResMgr.Instance:CreateEmptyGameObj("DrawObjPool", true)
    DrawObj.v_root:SetActive(false)
    DrawObj.v_root_transform = DrawObj.v_root.transform
    DrawObj.v_root_transform:SetParent(ResPoolMgr.Instance.v_pools_root_transform)
end
InitDrawObjPool()

local localPosition = Vector3(0, 0, 0)
local localRotation = Quaternion.Euler(0, 0, 0)
local localScale = Vector3(1, 1, 1)
local defaultLayer = UnityEngine.LayerMask.NameToLayer("Default")

---@return U3DObject
function DrawObj.Pop()
    local draw_obj = Next(DrawObj.obj_list)
    if draw_obj then
        draw_obj.transform.localPosition = localPosition
        draw_obj.transform.localRotation = localRotation
        draw_obj.transform.localScale = localScale
        draw_obj.gameObject.layer = defaultLayer
        draw_obj.gameObject:SetActive(true)
        draw_obj.transform:DOKill()
        DrawObj.obj_list[draw_obj] = nil
        DrawObj.obj_count = DrawObj.obj_count - 1
    else
        draw_obj = U3DObject(GameObject.New("DrawObj"))
        draw_obj.gameObject:AddComponent(TypeMovableObject)
    end
    return draw_obj
end

---@param draw_obj DrawObj
function DrawObj.Release(draw_obj)
    if DrawObj.obj_count <= 50 then
        if not IsNil(draw_obj.gameObject) then
            draw_obj.move_obj:Reset()
            draw_obj.transform:SetParent(DrawObj.v_root_transform, false)
            DrawObj.obj_list[draw_obj] = draw_obj
            DrawObj.obj_count = DrawObj.obj_count + 1
        end
    else
        GameObject.Destroy(draw_obj.gameObject)
    end
end

function DrawObj:__init(parent_obj, parent_trans)
    self.parent_obj = parent_obj
    self.root = DrawObj.Pop()
    self.root_transform = self.root.transform
    if parent_trans then
        self.root_transform:SetParent(parent_trans, false)
    end

    self.obj_type = SceneObjType.Unknown
end

function DrawObj:__delete()
    DrawObj.Release(self.root)
end

function DrawObj:SetObjType(obj_type)
    self.obj_type = obj_type
end

function DrawObj:IsDeleted()
    return self.root == nil
end

function DrawObj:GetRoot()
    return self.root
end

function DrawObj:GetPosition()
    if self:IsDeleted() then
        return
    end
    return self.root.transform.position
end

function DrawObj:SetPosition(pos)
    if self:IsDeleted() then
        return
    end
    self.root_transform.position = pos
end

function DrawObj:__CreatePart(part)
    local draw_part = DrawPart.Pop()
    draw_part:SetPart(part)
    draw_part:SetDrawObj(self)
    draw_part:SetParent(self.root)
    self:SetPart(part, draw_part)

    return draw_part
end

---@return DrawPart
function DrawObj:GetPart(part)
    local draw_part = self:TryGetPart(part)
    if not draw_part then
        draw_part = self:__CreatePart(part)
    end
    return draw_part
end

---@param obj DrawPart
function DrawObj:SetPart(part, obj)
    if not self.part_list then
        self.part_list = {}
    end
    self.part_list[part] = obj
end

---@return DrawPart
function DrawObj:TryGetPart(part)
    if self.part_list then
        return self.part_list[part]
    end
end

function DrawObj:RemoveModel(part)
    local draw_part = self:TryGetPart(part)
    if draw_part then
        draw_part:RemoveModel()
    end
end

function DrawObj:AnimOnMainPart(action, ...)
    action(self, SceneObjPart.Main, ...)
    --action(self,SceneObjPart.Weapon,...)
end

function DrawObj:SetAnimParamMain(type, key, value)
    local part_func = function(draw_obj, part, typo, k, v)
        ---@type DrawPart
        local draw_part = draw_obj:TryGetPart(part)
        if not draw_part then
            return
        end
        if typo == DrawObj.AnimParamType.Trigger then
            draw_part:SetTrigger(k)
        elseif typo == DrawObj.AnimParamType.Boolean then
            draw_part:SetBool(k, v)
        elseif typo == DrawObj.AnimParamType.Float then
            draw_part:SetFloat(k, v)
        elseif typo == DrawObj.AnimParamType.Integer then
            draw_part:SetInteger(k, v)
        elseif typo == DrawObj.AnimParamType.ResetTrigger then
            draw_part:ResetTrigger(k)
        end
    end
    self:AnimOnMainPart(part_func, type, key, value)
end

function DrawObj:MoveTo(pos, speed, callback)
    if self.root then
        self.root.move_obj:MoveTo(pos, speed, callback)
    end
end

function DrawObj:StopMove()
    if self.root then
        self.root.move_obj:StopMove()
    end
end

function DrawObj:RotateTo(pos, speed)
    if self.root then
        self.root.move_obj:RotateTo(pos, speed)
    end
end

function DrawObj:SetName(name)
    self.root.gameObject.name = name
end

function DrawObj:GetName()
    return self.root.gameObject.name
end

function DrawObj:GetTransform()
    return self.root_transform
end