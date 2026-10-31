---@meta

---@class ModsaInterface
ModsaInterface = {}


--- Returns the requested modsa VALUE.
---@param idx integer
---@return integer
function ModsaInterface:GetVALUE(idx) end

--- Sets the modsa VALUE to the given value.
---@param idx integer
---@param value integer
---@return nil
function ModsaInterface:SetVALUE(idx, value) end

--- Returns the requested ModUnitData.
---@param unit UnitModel
---@param dataID integer
---@return integer
function ModsaInterface:GetUnitData(unit, dataID) end

--- Sets ModUnitData to the given value.
---@param unit UnitModel
---@param dataID integer
---@param value integer
---@return integer
function ModsaInterface:SetUnitData(unit, dataID, value) end

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
function ModsaInterface.CreateBufPacket() end

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

--- Information Packet specific to Modular's ChangeTakeDamage TIMING.
---@class PacketChangeTakeDamage
---@field resultDmg integer
---@field preCalculatedDmg integer
---@field postfix_dmg integer Overwrite This
PacketChangeTakeDamage = {}

--- Creates and returns the current ActionModel (can be internally null).
---@return ActionModel
function PacketChangeTakeDamage:FetchAction() end

--- Creates and returns the current CoinModel (can be internally null).
---@return CoinModel
function PacketChangeTakeDamage:FetchCoin() end

--- Creates and returns the current DamageSourceType.
---@return DamageSourceType
function PacketChangeTakeDamage:FetchDmgSrcType() end

--- Creates and returns the current BufKeyword.
---@return BufKeyword
function PacketChangeTakeDamage:FetchBufKeyword() end

--- Creates and returns the current EventTiming.
---@return EventTiming
function ModsaInterface:FetchEventTiming() end

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

--- Can be instanced to store a DAMAGE_SOURCE_TYPE enum.
---@class DamageSourceType
DamageSourceType = {}

--- Creates a new DamageSourceType instance that internally stores a DAMAGE_SOURCE_TYPE enum by parsing a string.
---@param s string
---@return DamageSourceType
function DamageSourceType.CreateFromString(s) end

--- Creates a new DamageSourceType instance that internally stores a DAMAGE_SOURCE_TYPE enum by casting an int.
---@param integ integer
---@return DamageSourceType
function DamageSourceType.CreateFromInt(integ) end

--- Returns a string of the stored DAMAGE_SOURCE_TYPE.
---@return string
function DamageSourceType:ToString() end

--- 0 = "Player", 1 = "Enemy", 2 = "None".
---@alias UnitFaction
---| 0
---| 1
---| 2

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

--- Static Method Interface for BattleObjectManager.
---@class BattleObjectManager
BattleObjectManager = {}

---@param include_assistance boolean
---@param faction UnitFaction Use "None" for no filtering.
---@return UnitModel[]
function BattleObjectManager.GetAliveList(include_assistance, faction) end

---@param bufKeyword BufKeyword
---@param activeRound integer
---@param include_assistance boolean
---@param faction UnitFaction Use "None" for no filtering.
---@return UnitModel[]
function BattleObjectManager.GetAliveListByBuff(bufKeyword, activeRound, include_assistance, faction) end

--- Can be instanced to store a BATTLE_EVENT_TIMING enum.
---@class EventTiming
EventTiming = {}

--- Creates a new EventTiming instance that internally stores a BATTLE_EVENT_TIMING enum by parsing a string.
---@param s string
---@return EventTiming
function EventTiming.CreateFromString(s) end

--- Creates a new EventTiming instance that internally stores a BATTLE_EVENT_TIMING enum by casting an int.
---@param integ integer
---@return EventTiming
function EventTiming.CreateFromInt(integ) end

--- Returns a string of the stored BATTLE_EVENT_TIMING.
---@return string
function EventTiming:ToString() end

---@class UnitModel
UnitModel = {}

--- Checks if internal BattleUnitModel reference is null.
---@return boolean
function UnitModel:IsNull() end

--- Returns 0 if not an abno. Otherwise, returns 1 if Core, 2 if Part.
---@return integer
function UnitModel:WhatAmI() end

---@return integer
function UnitModel:UnitID() end

---@return integer
function UnitModel:InstanceID() end

---@return UnitFaction
function UnitModel:Faction() end

---@return integer
function UnitModel:HP() end

---@return integer
function UnitModel:MaxHP() end

---@return integer
function UnitModel:Speed() end

--- Retruns the pattern idx. Please for the love of god make sure this is an abno core OR part (at least for now)
---@return integer
function UnitModel:GetPatternIdx() end

--- Sets the pattern idx. Please for the love of god make sure this is an abno core OR part (at least for now)
---@param idx integer
---@return nil
function UnitModel:SetPatternIdx(idx) end

--- Attempts to retrieve a new UnitModel representing the abno core.
--- If this fails for any reason, like "it's not an abno" or "it's not a part", it returns itself and does not create anything.
---@return UnitModel
function UnitModel:FetchCore() end

--- Attempts to retrieve a list of UnitModel representing the abno's parts.
--- Also works if called on a Part.
--- If this fails for any reason, like "it's not an abno", it returns a list containing itself and does not create anything.
---@return UnitModel[]
function UnitModel:FetchParts() end

--- Retrieves a list of Active BufModel on this unit.
---@return BufModel[]
function UnitModel:FetchBufList() end

---@param bufKeyword BufKeyword
---@param main_only boolean
---@return integer
function UnitModel:BufCheckStack(bufKeyword, main_only) end

---@param bufCategory BufCategory
---@param exception_keywords BufKeyword[]
---@return integer
function UnitModel:BufCheckStackByCategory(bufCategory, exception_keywords) end

---@param bufKeyword BufKeyword
---@param main_only boolean
---@return integer
function UnitModel:BufCheckTurn(bufKeyword, main_only) end

---@param bufCategory BufCategory
---@param exception_keywords BufKeyword[]
---@return integer
function UnitModel:BufCheckTurnByCategory(bufCategory, exception_keywords) end

--- Internally Stores a BuffModel.
---@class BufModel
BufModel = {}

--- Checks if internal BuffModel reference is null.
---@return boolean
function BufModel:IsNull() end

--- Returns the Stack depending on the given activeRound
---@param activeRound integer
---@return integer
function BufModel:GetStack(activeRound) end

--- Returns the Turn depending on the given activeRound
---@param activeRound integer
---@return integer
function BufModel:GetTurn(activeRound) end

--- Returns a new BufKeyword that matches the bufModel's Main Keyword.
---@return BufKeyword
function BufModel:CreateMainBufKeyword() end

--- Returns a list of BufKeyword instances for each SubList Keyword.
---@return BufKeyword[]
function BufModel:CreateSubBufKeywordList() end

--- Returns a list of BufCategory instances for each Buf Category.
---@return BufCategory[]
function BufModel:CreateCategoryKeywordList() end

--- Internally Stores a BattleActionModel.
---@class ActionModel
ActionModel = {}

--- Checks if internal BattleActionModel reference is null.
---@return boolean
function ActionModel:IsNull() end

---@return UnitModel
function ActionModel:FetchUnitModel() end

---@return SkillModel
function ActionModel:FetchSkill() end

--- Internally Stores a vanilla SkillModel.
---@class SkillModel
SkillModel = {}

--- Checks if internal SkillModel reference is null.
---@return boolean
function SkillModel:IsNull() end

--- Internally Stores a vanilla CoinModel.
---@class CoinModel
CoinModel = {}

--- Checks if internal CoinModel reference is null.
---@return boolean
function CoinModel:IsNull() end

---@return integer
function CoinModel:GetRealCoinIndex() end