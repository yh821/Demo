---
--- Created by Hugo
--- DateTime: 2023/1/3 14:12
---

local class_name_table = {}
---@param class_type BaseClass
function GetClassName(class_type)
    local name = class_name_table[class_type]
    if nil == name then
        for k, v in pairs(_G) do
            if v == class_type then
                class_name_table[class_type] = k
                name = k
            end
        end
    end
    if nil == name then
        name = "unknow"
        class_name_table[class_type] = name
    end
    return name
end

local _class = {}
local lua_obj_count = 0

---@param super BaseClass
function BaseClass(super)
    -- 生成一个类类型
    ---@class BaseClass
    ---@field _class_type BaseClass
    local class_type = {}
    class_type.__init = false
    class_type.__delete = false
    class_type.super = super
    ---@overload fun():BaseClass
    class_type.New = function(...)
        lua_obj_count = lua_obj_count + 1
        -- 生成一个类对象
        ---@type BaseClass
        local obj = { _class_type = class_type }
        -- 在初始化之前注册基类方法
        setmetatable(obj, { __index = _class[class_type] })
        -- 初始化
        do
            local _ctor
            ---@param c BaseClass
            _ctor = function(c, ...)
                if c.super then
                    _ctor(c.super, ...)
                end
                if c.__init then
                    c.__init(obj, ...)
                end
            end
            _ctor(class_type, ...)
        end
        -- 注册一个delete方法
        ---@param self BaseClass
        obj.DeleteMe = function(self)
            if Status.IsEditor then
                if obj.__is_deleted__ then
                    print_error("重复调用DeleteMe", debug.traceback())
                end
            end
            obj.__is_deleted__ = true
            lua_obj_count = lua_obj_count - 1
            local now_super = self._class_type
            while now_super do
                if now_super.__delete then
                    now_super.__delete(self)
                end
                now_super = now_super.super
            end

            if obj.__game_obj_loaders then
                ReleaseGameObjLoaders(obj)
            end
            if obj.__res_loaders then
                ReleaseResLoaders(obj)
            end
            if obj.__delay_call_map then
                TimerQuest:CancelAllDelayCall(obj)
            end
        end

        return obj
    end

    local vt = {}
    _class[class_type] = vt

    local meta = {}
    meta.__newindex = function(t, k, v)
        vt[k] = v
    end
    meta.__index = vt
    setmetatable(class_type, meta)

    if super then
        setmetatable(vt, { __index = function(t, k)
            return _class[super][k]
        end })
    end

    return class_type
end