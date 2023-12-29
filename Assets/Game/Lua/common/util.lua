﻿---
--- Created by Hugo
--- DateTime: 2023/4/8 19:58
---

function IsNil(uobj)
    return uobj == nil or uobj:Equals(nil)
end

function IsNilOrEmpty(str)
    return str == nil or str == ""
end

--设置一个全局不可修改的空表
EmptyTable = {}
setmetatable(EmptyTable, {
    __newindex = function(t, k, v)
        print_error("please do not modify global EmptyTable!!!")
    end
})

function LastIndexOf(str, sub)
    local revered_sub = string.reverse(sub)
    local revered_str = string.reverse(str)
    local si, ei = string.find(revered_str, revered_sub)
    if si and ei then
        return -ei, -si
    end
end

function ToNumber(num)
    num = tonumber(num)
    if not num then
        return nil
    end
    local num_str = tostring(num)
    if num_str == "nan" or num_str == "-nan" or num_str == "inf" or num_str == "-inf" then
        return nil
    end
    return num
end

local _split_cache = {}

---@return string[]
function Split(str, splitter, use_cache)
    if use_cache and _split_cache[str] then
        return _split_cache[str]
    end

    local split_result = {}
    local index = 1

    while true do
        local si, ei = string.find(str, splitter, index)
        if not si then
            break
        end
        split_result[#split_result + 1] = string.sub(str, index, si - 1)
        index = ei + 1
    end

    if index <= string.len(str) then
        split_result[#split_result + 1] = string.sub(str, index)
    end

    if use_cache then
        _split_cache[str] = split_result
    end

    return split_result
end

function Next(t)
    local metatable = getmetatable(t)
    if metatable and rawget(metatable, "__next") then
        return metatable.__next(t)
    end
    return next(t)
end

function MathClamp(value, min, max)
    if value < min then
        return min
    end
    if value > max then
        return max
    end
    return value
end
