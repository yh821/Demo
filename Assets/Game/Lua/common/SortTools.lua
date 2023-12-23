﻿---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by admin.
--- DateTime: 2023/12/15 15:27
---

SortTools = {}

--升序排序函数
function SortTools.AscFunc(...)
    local params = { ... }
    local count = #params
    if count == 0 then
        return function(a, b)
            return a < b
        end
    elseif count == 1 then
        return function(a, b)
            return a[params[1]] < b[params[1]]
        end
    end

    return function(a, b)
        for _, v in ipairs(params) do
            if a[v] < b[v] then
                return true
            elseif a[v] > b[v] then
                return false
            end
        end
        return false
    end
end

--降序排序函数
function SortTools.DescFunc(...)
    local params = { ... }
    local count = #params
    if count == 0 then
        return function(a, b)
            return a > b
        end
    elseif count == 1 then
        return function(a, b)
            return a[params[1]] > b[params[1]]
        end
    end

    return function(a, b)
        for _, v in ipairs(params) do
            if a[v] > b[v] then
                return true
            elseif a[v] < b[v] then
                return false
            end
        end
        return false
    end
end

--升序排序
function SortTools.SortAsc(t, ...)
    table.sort(t, SortTools.AscFunc(...))
end

--降序排序
function SortTools.SortDesc(t, ...)
    table.sort(t, SortTools.DescFunc(...))
end