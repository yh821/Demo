local base = _G

------------------------------------------------------------------------
-- option handling, plays nice with --quiet option
------------------------------------------------------------------------

local option                    -- local reference to list of options
local src_file, dst_file             -- filenames
local tok_list, word_list, line_list -- token data
local level = 0

local ignore_global = {
	["UITween"] = true,
	["MsgAdapter"] = true,
}

local function print(...)
	-- handle quiet option
	--if option.QUIET then
	--	return
	--end
	base.print(...)
end

------------------------------------------------------------------------
-- initialization
------------------------------------------------------------------------

function init(opt, src_path, dst_path)
	option = opt
	src_file, dst_file = src_path, dst_path

	if option.PARAMS then
		level = tonumber(option.PARAMS[1]) or 0
	end
end

------------------------------------------------------------------------
-- message display, post-load processing, can return z
------------------------------------------------------------------------

function post_load(z)
	return z
end

------------------------------------------------------------------------
-- post-lexing processing, can work on lexer table output
------------------------------------------------------------------------

function post_lex(toklist, wordlist, linelist)
	tok_list, word_list, line_list = toklist, wordlist, linelist
end

------------------------------------------------------------------------
-- start check unused local var
------------------------------------------------------------------------
local function get_tok_word(pos)
	return tok_list[pos], word_list[pos]
end

local function save_file(file_name, content)
	print("save file:", file_name)
	local file = io.open(file_name, "wb")
	if not file then
		base.error("cannot open \"" .. file_name .. "\" for writing!")
	end
	local status = file:write(content)
	if not status then
		base.error("cannot write to \"" .. file_name .. "\"!")
	end
	file:close()
end

local global_dict = {}
local function init_global_list(global_info)
	for i, v in ipairs(global_info) do
		global_dict[v.name] = v
	end
end

local function is_invalid_tok(tok)
	return tok == TOK.SPACE or tok == TOK.EOL or tok == TOK.COMMENT or tok == TOK.LCOMMENT
end

local valid_count = 0
local get_valid_word
get_valid_word = function(pos, step, count)
	local tok, word = get_tok_word(pos)
	if tok == nil or word == nil then
		return word, pos, tok
	end
	if is_invalid_tok(tok) then
		return get_valid_word(pos + step, step, count)
	elseif valid_count < count then
		valid_count = valid_count + 1
		return get_valid_word(pos + step, step, count)
	else
		return word, pos, tok
	end
end

local function get_next_word(start_pos, count)
	count = count or 1
	valid_count = 0
	return get_valid_word(start_pos, 1, count)
end

local function get_prev_word(start_pos, count)
	count = count or 1
	valid_count = 0
	return get_valid_word(start_pos, -1, count)
end

local function get_connect(start_pos, end_pos)
	local connect = {}
	for i = start_pos, end_pos do
		connect[#connect + 1] = word_list[i]
	end
	return table.concat(connect)
end

--start with var
local function is_multi_var(pos)
	local n_word = get_next_word(pos)
	if n_word == "," then
		return true
	end
	local p_word = get_prev_word(pos)
	if p_word == "," then
		return true
	end
	return false
end

--start with "function"
local function get_func_end_pos(func_start_pos)
	local depth = 1
	local pos = func_start_pos + 1
	local tok, word = get_tok_word(pos)
	while pos <= #tok_list do
		if tok == TOK.KEYWORD then
			if word == "function" or word == "if" or word == "for" or word == "while" or word == "repeat" then
				depth = depth + 1
			elseif word == "end" or word == "until" then
				depth = depth - 1
			end
			if depth == 0 then
				break
			end
		end
		pos = pos + 1
		tok, word = get_tok_word(pos)
	end
	return pos
end

local op_dict = { ["{"] = "}", ["["] = "]", ["("] = ")" }
--start with "{" or "[" or "("
local function get_op_end_pos(start_pos, start_op)
	local end_op = op_dict[start_op]
	local depth = 1
	local pos = start_pos + 1
	local word = word_list[pos]
	while pos <= #word_list do
		if word == start_op then
			depth = depth + 1
		elseif word == end_op then
			depth = depth - 1
		end
		if depth == 0 then
			break
		end
		pos = pos + 1
		word = word_list[pos]
	end
	return pos
end

--start with "="
local function get_assign_end_pos(start_pos)
	local n_word, n_pos = get_next_word(start_pos)
	if n_word == "function" then
		return get_func_end_pos(n_pos)
	elseif n_word == "{" then
		return get_op_end_pos(n_pos, n_word)
	end
	local last_type = 0
	local pos = start_pos + 1
	local last_pos = start_pos
	local tok, word = get_tok_word(pos)
	while pos <= #tok_list do
		if not is_invalid_tok(tok) then
			if tok == TOK.KEYWORD then
				if word == "true" or word == "false" or word == "nil" then
					last_type = 1
				elseif word == "and" or word == "or" or word == "not" then
					last_type = 0
				else
					break
				end
			elseif tok == TOK.NAME or tok == TOK.NUMBER or tok == TOK.STRING or tok == TOK.LSTRING then
				if last_type > 0 then
					break
				end
				last_type = 1
			elseif tok == TOK.OP then
				if word == "(" or word == "[" or word == "{" then
					pos = get_op_end_pos(pos, word)
					last_type = 2
				else
					last_type = 0
				end
			elseif tok == TOK.EOS then
				break
			end
			last_pos = pos
		end
		pos = pos + 1
		tok, word = get_tok_word(pos)
	end
	return last_pos
end

local function set_assign_list(node, pos, is_define)
	local p_word, p_pos = get_prev_word(pos)
	local n_word, n_pos = get_next_word(pos)
	local is_func = p_word == "function"
	local is_assign = n_word == "="
	if not is_define and not is_assign and not is_func then
		node.is_used = true
		return
	end
	if is_assign then
		local nn_word = get_next_word(pos, 2)
		if global_dict[nn_word] and ignore_global[nn_word] then
			node.is_used = true
			return
		end
	end
	local start_pos = node.start_pos
	local end_pos = pos
	if is_func or is_assign then
		start_pos = is_define and node.start_pos or pos
		end_pos = is_func and get_func_end_pos(p_pos) or get_assign_end_pos(n_pos)
	end

	local connect = get_connect(start_pos, end_pos)
	local s, e, n = string.find(connect, "=%s-([%w_]+)$")
	if s and n and global_dict[n] then
		node.is_used = true
		return
	end
	if level >= 2 then
		--match  rect:DOAnchorPosX() --canvas_group:DoAlpha()
		s = string.find(connect, "[%w_]+:D[Oo][%w_]+%(.*%)")
		if s then
			node.is_used = true
			return
		end
	else
		--match  a:Test1() --B.test2()
		s = string.find(connect, "[%w_]+[%.:][%w_]+%(.*%)")
		if s then
			node.is_used = true
			return
		end
	end
	if not node.assign_list then
		node.assign_list = {}
	end
	local start_line = line_list[start_pos]
	local end_line = line_list[end_pos]
	local _, end_n_pos = get_next_word(end_pos)
	local end_n_line = line_list[end_n_pos]
	if start_line == end_line and end_line ~= end_n_line then
		node.assign_list[start_pos] = 3
		node.assign_list[end_pos + 1] = 4
	else
		node.assign_list[start_pos] = 1
		node.assign_list[end_pos + 1] = 2
	end
end

local function get_comment(flag)
	if flag == 1 then
		return "--[["
	elseif flag == 2 then
		return "]]"
	elseif flag == 3 then
		return "--"
	end
end

local function init_local_list(local_info)
	local unused_node_list = {}
	for _, v in ipairs(local_info) do
		if not v.isself then
			local decl = v.decl
			local p_word, p_pos = get_prev_word(decl)
			local is_func = p_word == "function"
			if p_word == "local" or is_func then
				local node = { pos = decl }
				if is_func then
					_, node.start_pos = get_prev_word(decl, 2)
				else
					node.start_pos = p_pos
				end
				set_assign_list(node, decl, true)
				for _, ref in ipairs(v.xref) do
					if is_multi_var(ref) then
						node.is_used = true
					end
					if node.is_used then
						break
					end
					if ref ~= decl then
						set_assign_list(node, ref, false)
					end
				end
				if not node.is_used then
					table.insert(unused_node_list, node)
				end
			end
		end
	end
	local need_comment_pos = {}
	for _, node in ipairs(unused_node_list) do
		if node.assign_list then
			for pos, flag in pairs(node.assign_list) do
				need_comment_pos[pos] = flag
			end
		end
	end
	return need_comment_pos
end

------------------------------------------------------------------------
-- post-parsing processing, gives globalinfo, localinfo
------------------------------------------------------------------------

function post_parse(global_info, local_info)
	local results = {}
	local function add(s)
		results[#results + 1] = s
	end

	--init global
	init_global_list(global_info)

	--init local
	local comment_list = init_local_list(local_info)

	--del local
	local k = next(comment_list)
	if k then
		local cur_pos = 1
		while cur_pos <= #tok_list do
			local comment = get_comment(comment_list[cur_pos])
			if comment then
				add(comment)
			end
			add(word_list[cur_pos])
			cur_pos = cur_pos + 1
		end
		save_file(src_file, table.concat(results))
	end

	option.EXIT = true
end

return base.getfenv()
