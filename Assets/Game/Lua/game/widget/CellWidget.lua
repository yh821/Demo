---
--- Created by Hugo
--- DateTime: 2024/10/9 20:08
---

---@class CellWidget : BaseWidget
CellWidget = CellWidget or BaseClass(BaseWidget)

function CellWidget:__init()
    self.is_select = false
    self.ignore_data_to_select = false
    self.select_index = 0
end

function CellWidget:__delete()
    self.data = nil
    self.ignore_data_to_select = false
    self.need_change = nil
    self:ReleaseCallBack()
end

function CellWidget:GetView()
    return self.view
end

function CellWidget:GetData()
    return self.data
end

function CellWidget:GetIndex()
    return self.index
end

function CellWidget:SetIndex(index)
    self.index = index
end

function CellWidget:GetName()
    return self.name
end

function CellWidget:SetName(name)
    self.name = name
end

function CellWidget:SetData(data)
    self.data = data
    if self.is_loaded or self.is_use_obj_pool then
        self:OnFlush()
    else
        self:Flush()
    end
end

function CellWidget:SetIgnoreDataToSelect(value)
    if self.ignore_data_to_select and not value then
        self:SetSelect(false)
    end
    self.ignore_data_to_select = value
end

function CellWidget:SetPosition(x, y)
    self.view.transform:SetPosition(x, y, 0)
end

function CellWidget:SetLocalPosition(x, y)
    self.view.transform:SetLocalPosition(x, y, 0)
end

function CellWidget:SetAnchoredPosition(x, y)
    self.view.rect:SetAnchoredPosition(x, y)
end

function CellWidget:SetSizeDelta(x, y)
    self.view.rect:SetSizeDelta(x, y)
end

function CellWidget:SetVisible(value)
    if value ~= nil and self.view then
        self.view:SetActive(value)
    end
end

function CellWidget:AddClickListener(callback, is_toggle)
    if not callback then
        return
    end
    self.need_click_listen = false
    self.click_callback = callback
    local func = function()
        self:OnClick()
    end
    if is_toggle then
        self.view.toggle:AddClickListener(func)
    elseif self.view.button then
        self.view.button:AddClickListener(func)
    end
end