---@meta

---@class ModsaInterface
ModsaInterface = {}


---@param find_instruction string
---@return UnitModel
function ModsaInterface:FetchSingleUnit(find_instruction) end

---@param find_instruction string
---@return UnitModel[]
function ModsaInterface:FetchUnitList(find_instruction) end

---@param targetModel UnitModel
---@param giver UnitModel
---@param bufKeyword BufKeyword
---@param stack integer
---@param turn integer
---@param activeRound_list integer[]
---@param bufPacket BufPacket
---@return nil
function ModsaInterface:Buf(targetModel, giver, bufKeyword, stack, turn, activeRound_list, bufPacket) end

--- Creates and returns a default BufPacket.
---@return BufPacket
function ModsaInterface:CreateBufPacket() end

--- Stores information and settings for ModsaInterface.Buf().
---@class BufPacket
---@field mode_action BPModeAction
---@field mode_infliction BPModeInfliction
---@field mode_source BPModeSource
BufPacket = {}

---@alias BPModeAction
---| "normal"
---| "nullaction"
---| "oppoaction"

---@alias BPModeInfliction
---| "give"
---| "remove"
---| "spend"

---@alias BPModeSource
---| "unit"
---| "skill"
---| "coin"
---| "buf"
---| "passive"

--- Can be instanced to store a BUFF_UNIQUE_KEYWORD enum.
---@class BufKeyword
BufKeyword = {}

--- Creates a new BufKeyword instance that internally stores a BUFF_UNIQUE_KEYWORD enum by parsing a string.
---@param keyword_s string
---@return BufKeyword
function BufKeyword.CreateFromString(keyword_s) end

--- Creates a new BufKeyword instance that internally stores a BUFF_UNIQUE_KEYWORD enum by casting an int.
---@param keyword_int integer
---@return BufKeyword
function BufKeyword.CreateFromInt(keyword_int) end

--- Returns a string of the stored BUFF_UNIQUE_KEYWORD.
---@return string
function BufKeyword:ToString() end

--- Can be instanced to store a BUFF_CATEGORY_KEYWORD enum.
---@class BufCategory
BufCategory = {}

--- Creates a new BufCategory instance that internally stores a BUFF_CATEGORY_KEYWORD enum by parsing a string.
---@param category_s string
---@return BufCategory
function BufCategory.CreateFromString(category_s) end

--- Creates a new BufCategory instance that internally stores a BUFF_CATEGORY_KEYWORD enum by casting an int.
---@param category_int integer
---@return BufCategory
function BufCategory.CreateFromInt(category_int) end

--- Returns a string of the stored BUFF_CATEGORY_KEYWORD.
---@return string
function BufCategory:ToString() end

--- Static Method Interface for BattleUnitBuffManager.
---@class BufManager
BufManager = {}

--- Checks if a keyword is the same or comparable to another.
--- Has options for comparison.
---@param keyword_main BufKeyword
---@param keyword_another BufKeyword
---@param mode_s BufManagerModeCompare1
---@return boolean
function BufManager.CompareBufKeyword(keyword_main, keyword_another, mode_s) end

---@alias BufManagerModeCompare1
---| "main_only"
---| "main_and_sub"

--- Checks if a buf keyword contains a buf category.
---@param keyword_main BufKeyword
---@param category_another BufCategory
---@return boolean
function BufManager.KeywordHasCategory(keyword_main, category_another) end

---@class UnitModel
---@field name string
UnitModel = {}

--- Checks if internal BattleUnitModel reference is null.
---@return boolean
function UnitModel:IsNull() end

--- Returns 0 if not an abno. Otherwise, returns 1 if Core, 2 if Part.
---@return integer
function UnitModel:WhatAmI() end

---@return integer
function UnitModel:HP() end

---@return integer
function UnitModel:MaxHP() end

---@return integer
function UnitModel:Speed() end