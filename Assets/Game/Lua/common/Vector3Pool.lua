﻿---
--- Created by Hugo
--- DateTime: 2023/4/23 20:52
---

---@class Vector3Tool
Vector3Pool = Vector3Pool or {}
local vec3_auto_die_cache = {}
local vec3_died_cache = {}

--用于赋值给transform的无CG Vector2/Vector3
Vector3Pool._temp_vector2 = Vector2(0, 0)
Vector3Pool._temp_vector3 = Vector3(0, 0, 0)
Vector3Pool._temp_vector4 = Vector4(0, 0, 0, 0)

Vector3Pool.zero = Vector3(0, 0, 0)
Vector3Pool.one = Vector3(1, 1, 1)
Vector3Pool.rotate = Quaternion.Euler(0, 0, 0)

function Vector3Pool.GetTemp(x, y, z, w)
    if w then
        Vector3Pool._temp_vector4.x = x
        Vector3Pool._temp_vector4.y = y
        Vector3Pool._temp_vector4.z = z
        Vector3Pool._temp_vector4.w = w
        return Vector3Pool._temp_vector4
    elseif z then
        Vector3Pool._temp_vector3.x = x
        Vector3Pool._temp_vector3.y = y
        Vector3Pool._temp_vector3.z = z
        return Vector3Pool._temp_vector3
    else
        Vector3Pool._temp_vector2.x = x
        Vector3Pool._temp_vector2.y = y
        return Vector3Pool._temp_vector2
    end
end

function Vector3Pool.Get(x, y, z)
    local v = next(vec3_died_cache)
    if v then
        v.x = x or 0
        v.y = y or 0
        v.z = z or 0
        vec3_died_cache[v] = nil
    else
        v = Vector3.zero
    end

    vec3_auto_die_cache[v] = v
    return v
end

function Vector3Pool.Update(unscaledTime, unscaledDeltaTime)
    for k, v in pairs(vec3_auto_die_cache) do
        vec3_died_cache[k] = v
        vec3_auto_die_cache[k] = nil
    end
end

function Vector3Pool.Add(v1, v2)
    return Vector3Pool.Get(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z)
end

function Vector3Pool.Sub(v1, v2)
    return Vector3Pool.Get(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z)
end

function Vector3Pool.Mul(v, f)
    return Vector3Pool.Get(v.x * f, v.y * f, v.z * f)
end

function Vector3Pool.Dot(v1, v2)
    return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z
end

function Vector3Pool.Cross(v1, v2)
    local x = v1.y * v2.z - v1.z * v2.y
    local y = v1.z * v2.x - v1.x * v2.z
    local z = v1.x * v2.y - v1.y * v2.x
    return Vector3Pool.Get(x, y, z)
end

function Vector3Pool.Distance(v, is_sqrt)
    if is_sqrt == false then
        return v.x * v.x + v.y * v.y + v.z * v.z
    end
    return math.sqrt(v.x * v.x + v.y * v.y + v.z * v.z)
end

function Vector3Pool.Normalize(v)
    local dis = Vector3Pool.Distance(v)
    if dis == 0 then
        return Vector3Pool.Get(0, 0, 1)
    end
    return Vector3Pool.Get(v.x / dis, v.y / dis, v.z / dis)
end

local _up = Vector3.up
local _next = { 2, 3, 1 }
local q = { 0, 0, 0 }
local matrix = {
    { 0, 0, 0 },
    { 0, 0, 0 },
    { 0, 0, 0 }
}
function Vector3Pool.LookRotation(forward, up)
    local mag = Vector3Pool.Distance(forward)
    if mag < 1e-6 then
        return nil
    end
    forward = Vector3Pool.Mul(forward, 1 / mag)
    up = up or _up
    local right = Vector3Pool.Cross(up, forward)
    right = Vector3Pool.Normalize(right)
    up = Vector3Pool.Cross(forward, right)
    right = Vector3Pool.Cross(up, forward)

    local t = right.x + up.y + forward.z
    if t > 0 then
        local x, y, z, w
        t = t + 1
        local s = 0.5 / math.sqrt(t)
        w = s * t
        x = (up.z - forward.y) * s
        y = (forward.x - right.z) * s
        z = (right.y - up._next) * s
        local ret = Quaternion.New(x, y, z, w)
        ret:SetNormalize()
        return ret
    else
        matrix[1][1] = right.x
        matrix[1][2] = up.x
        matrix[1][3] = forward.x
        matrix[2][1] = right.y
        matrix[2][2] = up.y
        matrix[2][3] = forward.y
        matrix[3][1] = right.z
        matrix[3][2] = up.z
        matrix[3][3] = forward.z
        local i = 1
        if up.y > right.x then
            i = 2
        end
        if forward.z > matrix[i][i] then
            i = 3
        end
        local j = _next[i]
        local k = _next[j]

        local r = matrix[i][i] - matrix[j][j] - matrix[k][k] + 1
        local s = 0.5 / math.sqrt(r)
        q[i] = r * s
        local w = (matrix[k][j] - matrix[j][k]) * s
        q[j] = (matrix[j][i] - matrix[i][j]) * s
        q[k] = (matrix[k][i] - matrix[i][k]) * s

        local ret = Quaternion.New(q[1], q[2], q[3], w)
        ret:SetNormalize()
        return ret
    end
end