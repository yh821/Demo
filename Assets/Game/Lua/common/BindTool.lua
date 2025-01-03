﻿---
--- Created by Hugo
--- DateTime: 2023/4/26 17:54
---

BindTool = BindTool or {}

function BindTool.UnPack(params, count, i, ...)
    if i >= count then
        if i == count then
            return params[i], ...
        end
        return ...
    end
    return params[i], BindTool.UnPack(params, count, i + 1, ...)
end

function BindTool.Bind(func, ...)
    if type(func) ~= "function" then
        print_error("Bind param is not function!")
        return function()
        end
    end

    local count = select('#', ...)
    local params = count == 0 and EmptyTable or { ... }
    local new_func = nil

    if count == 0 then
        new_func = function(...)
            return func(...)
        end
    elseif count == 1 then
        new_func = function(...)
            return func(params[1], ...)
        end
    elseif count == 2 then
        new_func = function(...)
            return func(params[1], params[2], ...)
        end
    else
        new_func = function(...)
            return func(BindTool.UnPack(params, count, 1, ...))
        end
    end

    return new_func
end

function BindTool.Bind1(func, arg1)
    if type(func) ~= "function" then
        print_error("Bind param is not function!")
        return function()
        end
    end
    local new_func = function(...)
        return func(arg1, ...)
    end
    return new_func
end