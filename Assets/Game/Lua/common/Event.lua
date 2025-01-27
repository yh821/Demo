﻿---
--- Created by Hugo
--- DateTime: 2023/4/26 21:23
---

---@class Event : BaseClass
Event = Event or BaseClass()

function Event:__init(id)
    self.event_id = id
    self.bind_id_count = 0
    self.bind_count = 0
    self.event_func_list = {}
end

function Event:Fire(args)
    if UNITY_EDITOR then
        for i, v in pairs(self.event_func_list) do
            v(unpack(args))
        end
    else
        for i, v in pairs(self.event_func_list) do
            local s, e = pcall(v, unpack(args))
            if not s then
                print_error("Event:", self.event_id, ", Invoke Error:", e)
            end
        end
    end
end

function Event:UnBind(obj)
    if obj.event_id == self.event_id then
        self.bind_count = self.bind_count - 1
        self.event_func_list[obj.bind_id] = nil
    end
end

function Event:Bind(func)
    self.bind_count = self.bind_count + 1
    self.bind_id_count = self.bind_id_count + 1
    local obj = { event_id = self.event_id, bind_id = self.bind_id_count }
    self.event_func_list[obj.bind_id] = func
    return obj
end

function Event:GetBindCount()
    return self.bind_count
end
