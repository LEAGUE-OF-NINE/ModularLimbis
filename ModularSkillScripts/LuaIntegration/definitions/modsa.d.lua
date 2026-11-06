---@meta

---@class ModularUtil
ModularUtil = {}

--- Randomizes a LuaTable's items (by reference).
---@param table table
function ModularUtil.ShuffleList(table) end

---Functionally identical to Modular's random() Getter method.
---@param minRoll integer (inclusive)
---@param maxRoll integer (inclusive)
---@return integer
---@nodiscard
function ModularUtil.GetterRandom(minRoll, maxRoll) end

--- Returns the instance of the lua table for encounter data.
--- This method does not have a "Set" counterpart, as it is a reference to an object, not a copy.
--- This is a LuaTable, as in a Dictionary. You can look up how to handle dictionaries.
--- This data will be available to any script until the Encounter ends and a new one starts, at which point all data in it will be cleared.
---@return table
---@nodiscard
function ModularUtil.FetchComplexEncounterData() end

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

--- Sets the selected Modsa ADDER.
---@param adder_type ModsaADDER
---@param amount_set integer
function ModsaInterface:ADDERSet(adder_type, amount_set) end

---@alias ModsaADDER
---| "base"
---| "clash"
---| "scale"
---| "final"
---| "dmgmult"
---| "dmgadd"
---| "critmult"
---| "critchance"
---| "headschance"

---@return UnitModel
---@nodiscard
function ModsaInterface:FetchModsaUnitModel() end

---@return BufModel
---@nodiscard
function ModsaInterface:FetchModsaBufModel() end

---@return SkillModel
---@nodiscard
function ModsaInterface:FetchModsaSkillModel() end

---@return ActionModel
---@nodiscard
function ModsaInterface:FetchModsaActionModel() end

---@return ActionModel
---@nodiscard
function ModsaInterface:FetchModsaActionModelOpponent() end

---@return CoinModel
---@nodiscard
function ModsaInterface:FetchModsaCoinModel() end

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

--- Information Packet specific to Modular's OverwriteGiveBuf and OverwriteGainBuf TIMINGs.
---@class PacketOverwriteGiveBuf
---@field info_abilitySrcType AbilitySourceType Display Only
---@field postfix_stack integer Overwrite This
---@field postfix_turn integer Overwrite This
---@field postfix_activeRound integer Overwrite This
---@field postfix_bufKeyword BufKeyword Overwrite This
PacketOverwriteGiveBuf = {}

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

--- Can be instanced to store a ABILITY_SOURCE_TYPE enum.
---@class AbilitySourceType
AbilitySourceType = {}

--- Creates a new AbilitySourceType instance that internally stores a ABILITY_SOURCE_TYPE enum by parsing a string.
---@param s string
---@return AbilitySourceType
function AbilitySourceType.CreateFromString(s) end

--- Creates a new AbilitySourceType instance that internally stores a ABILITY_SOURCE_TYPE enum by casting an int.
---@param integ integer
---@return AbilitySourceType
function AbilitySourceType.CreateFromInt(integ) end

--- Returns a string of the stored ABILITY_SOURCE_TYPE.
---@return string
function AbilitySourceType:ToString() end

--- Can be instanced to store a RESERVE_CUT_IN_ACTION_TYPE enum.
---@class ReserveCutInActionType
ReserveCutInActionType = {}

--- Creates a new ReserveCutInActionType instance that internally stores a RESERVE_CUT_IN_ACTION_TYPE enum by parsing a string.
---@param s string
---@return ReserveCutInActionType
function ReserveCutInActionType.CreateFromString(s) end

--- Creates a new ReserveCutInActionType instance that internally stores a RESERVE_CUT_IN_ACTION_TYPE enum by casting an int.
---@param integ integer
---@return ReserveCutInActionType
function ReserveCutInActionType.CreateFromInt(integ) end

--- Returns a string of the stored RESERVE_CUT_IN_ACTION_TYPE.
---@return string
function ReserveCutInActionType:ToString() end

--- Prints all in-game RESERVE_CUT_IN_ACTION_TYPE enum values to the Console.
---@return nil
function ReserveCutInActionType:PrintAllEnumValues() end

--- Can be instanced to store a SKILL_TARGET_TYPE enum.
---@class SkillTargetType
SkillTargetType = {}

--- Creates a new SkillTargetType instance that internally stores a SKILL_TARGET_TYPE enum by parsing a string.
---@param s string
---@return SkillTargetType
function SkillTargetType.CreateFromString(s) end

--- Creates a new SkillTargetType instance that internally stores a SKILL_TARGET_TYPE enum by casting an int.
---@param integ integer
---@return SkillTargetType
function SkillTargetType.CreateFromInt(integ) end

--- Returns a string of the stored SKILL_TARGET_TYPE.
---@return string
function SkillTargetType:ToString() end

--- Prints all in-game SKILL_TARGET_TYPE enum values to the Console.
---@return nil
function SkillTargetType:PrintAllEnumValues() end

--- Can be instanced to store an ATTRIBUTE_TYPE enum.
---@class AttributeType
AttributeType = {}

--- Creates a new AttributeType instance that internally stores a ATTRIBUTE_TYPE enum by parsing a string.
---@param s string
---@return AttributeType
function AttributeType.CreateFromString(s) end

--- Creates a new AttributeType instance that internally stores a ATTRIBUTE_TYPE enum by casting an int.
---@param integ integer
---@return AttributeType
function AttributeType.CreateFromInt(integ) end

--- Returns a string of the stored ATTRIBUTE_TYPE.
---@return string
function AttributeType:ToString() end

--- Returns an integer of the stored ATTRIBUTE_TYPE enum value.
---@return integer
function AttributeType:ToInt() end

--- Prints all in-game ATTRIBUTE_TYPE enum values to the Console.
---@return nil
function AttributeType:PrintAllEnumValues() end

--- Compares this and another.
---@param other AttributeType
---@return boolean
function AttributeType:IsEqualTo(other) end

--- Can be instanced to store an ATK_BEHAVIOUR enum.
---@class AtkType
AtkType = {}

--- Creates a new AtkType instance that internally stores a ATK_BEHAVIOUR enum by parsing a string.
---@param s string
---@return AtkType
function AtkType.CreateFromString(s) end

--- Creates a new AtkType instance that internally stores a ATK_BEHAVIOUR enum by casting an int.
---@param integ integer
---@return AtkType
function AtkType.CreateFromInt(integ) end

--- Returns a string of the stored ATK_BEHAVIOUR.
---@return string
function AtkType:ToString() end

--- Returns an integer of the stored ATK_BEHAVIOUR enum value.
---@return integer
function AtkType:ToInt() end

--- Prints all in-game ATK_BEHAVIOUR enum values to the Console.
---@return nil
function AtkType:PrintAllEnumValues() end

--- Compares this and another.
---@param other AtkType
---@return boolean
function AtkType:IsEqualTo(other) end

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

---@param unitOrInstID UnitModel|integer
---@return UnitView
---@nodiscard
function BattleObjectManager.GetView(unitOrInstID) end

--- Static Method Interface for BattleActionModelManager.
---@class ActionModelManager
ActionModelManager = {}

--- Returns a COPIED list (not reference) of all actions during battle. Modifying this list does NOT modify the original. Use SetActionList instead.
---@return ActionModel[]
function ActionModelManager.FetchActionList() end

--- Overwrites the manager's actionList.
---@param actionList ActionModel[]
---@return nil
function ActionModelManager.SetActionList(actionList) end

--- Remove the action through the manager.
---@param action_remove ActionModel
---@return nil
function ActionModelManager.RemoveAction(action_remove) end

--- Returns a COPIED list (not reference) of all duels during battle. Modifying this list does NOT modify the original. Use SetDuelList instead.
---@return DuelModel[]
function ActionModelManager.FetchDuelList() end

--- Overwrites the manager's duelList.
---@param duelList DuelModel[]
---@return nil
function ActionModelManager.SetDuelList(duelList) end

---@param action ActionModel
---@return boolean
function ActionModelManager.IsDuelForThisOne(action) end

---@param action1 ActionModel
---@param action2 ActionModel
---@return boolean
function ActionModelManager.IsDuelBetweenTheseTwo(action1, action2) end

--- Check the returned value by using DuelModel:IsNull()
---@param action ActionModel
---@return DuelModel
function ActionModelManager.FetchDuelFromAction(action) end

---@param action ActionModel
---@return nil
function ActionModelManager.RemoveDuelByAction(action) end

---@param action1 ActionModel
---@param action2 ActionModel
---@return nil
function ActionModelManager.AddDuel(action1, action2) end

---@param action ActionModel
---@return nil
function ActionModelManager.AddAction(action) end

---@param action ActionModel
---@param skillTargetType SkillTargetType
---@param lastOrFirst ModeLastOrFirst
---@return nil
function ActionModelManager.CutInAction(action, skillTargetType, lastOrFirst) end

---@alias ModeLastOrFirst
---| "Last"
---| "First"

---@param action ActionModel
---@param otherAction ActionModel
---@param skillTargetType SkillTargetType
---@return nil
function ActionModelManager.CutInActionPrevOtherAction(action, otherAction, skillTargetType) end

---@param action ActionModel
---@param first boolean
---@return nil
function ActionModelManager.CutInDefenseActionForcely(action, first) end

---@param uniqueCutIn UniqueCutInTiming
---@param action ActionModel
---@return nil
function ActionModelManager.AddUniqueCutIn(uniqueCutIn, action) end

---@alias UniqueCutInTiming
---| "None"
---| "JudgeDuelResult"

---@param reserveType ReserveCutInActionType
---@param action ActionModel
---@param removeDuel boolean Default: false
---@return nil
function ActionModelManager.ReserveToCutInActionUntilStartRunBattle(reserveType, action, removeDuel) end

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
---@nodiscard
function UnitModel:Speed() end

function UnitModel:RefreshSpeed() end

function UnitModel:RefreshState() end

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

--- Damages another unit and returns a list of integer values.
---* [1] = The damage return value.
---* [2] = out hpDamage.
---* [3] = out damageToShield.
---@param target UnitModel
---@param value integer
---@param timing EventTiming
---@param dmgSrcType DamageSourceType
---@param actionOrNull ActionModel?
---@param coinOrNull CoinModel?
---@return integer[]
function UnitModel:GiveAbsHpDamage(target, value, timing, dmgSrcType, actionOrNull, coinOrNull) end

--- Damages THIS unit (with optional attacker) and returns a list of integer values.
---* [1] = The damage return value.
---* [2] = out hpDamage.
---* [3] = out damageToShield.
---@param attackerOrNull UnitModel?
---@param value integer
---@param timing EventTiming
---@param dmgSrcType DamageSourceType
---@param actionOrNull ActionModel?
---@param coinOrNull CoinModel?
---@param relatedUnits UnitModel[]?
---@param bufKeyword BufKeyword?
---@param deactivePassedBreakSection boolean? (Default = false)
---@param ignoreShield boolean? (Default = false)
---@param ignorePassedBreakSection boolean? (Default = false)
---@return integer[]
function UnitModel:TakeAbsHpDamage(attackerOrNull, value, timing, dmgSrcType, actionOrNull, coinOrNull, bufKeyword,
                                   relatedUnits, deactivePassedBreakSection, ignoreShield, ignorePassedBreakSection)
  ---@diagnostic disable-next-line: missing-return
end

---@return UnitDataModel
function UnitModel:FetchDataModel() end

---@return ResistDetail
---@nodiscard
function UnitModel:FetchResistDetail() end

---@param sorted boolean
---@return ActionModel[]
---@nodiscard
function UnitModel:FetchActionModelList(sorted) end

---@param permanent_only boolean
---@return SinActionModel[]
---@nodiscard
function UnitModel:FetchSinActionList(permanent_only) end

---@param sinAction SinActionModel
function UnitModel:AddSinAction(sinAction) end

---@return SinActionModel
---@nodiscard
function UnitModel:AddNewSinActionModel() end

---@param skillID integer
---@return ActionModel
---@nodiscard
function UnitModel:AddNewSinActionModelWithSkillID(skillID) end

---@param include_ego boolean
---@return PassiveModel[]
---@nodiscard
function UnitModel:FetchPassives(include_ego) end

---@class UnitDataModel
UnitDataModel = {}

--- Checks if internal UnitDataModel reference is null.
---@return boolean
function UnitDataModel:IsNull() end

---@return UnitStatic
---@nodiscard
function UnitDataModel:FetchStatic() end

---@param skillID integer
---@return SkillModel[]
---@nodiscard
function UnitDataModel:FetchSkillModel(skillID) end

---@return integer[]
---@nodiscard
function UnitDataModel:FetchAllSkillID() end

--- Internally Stores a vanilla UnitStaticData.
---@class UnitStatic
UnitStatic = {}

--- Checks if internal UnitStatic reference is null.
---@return boolean
---@nodiscard
function UnitStatic:IsNull() end

--- Internally Stores a vanilla PassiveModel.
---@class PassiveModel
PassiveModel = {}

--- Checks if internal PassiveModel reference is null.
---@return boolean
---@nodiscard
function PassiveModel:IsNull() end

---@return integer
---@nodiscard
function PassiveModel:PassiveID() end

--- Returns all of this thing's modsas.
---@param fast_no_copy boolean (Default = false)
---@param check_active boolean (Default = true for checking if the passive's requirements are fulfilled)
---@return ModsaInterface[]
function PassiveModel:FetchAllModsa(fast_no_copy, check_active) end

--- Internally Stores a BuffModel.
---@class BufModel
BufModel = {}

--- Checks if internal BuffModel reference is null.
---@return boolean
function BufModel:IsNull() end

--- Returns all of this thing's modsas.
---@param fast_no_copy boolean (Default = false)
---@return ModsaInterface[]
function BufModel:FetchAllModsa(fast_no_copy) end

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

---@param victim UnitModel
---@param attacker UnitModel
---@param loseStack integer
---@param loseTurn integer
---@param timing EventTiming
---@param sinType AttributeType
---@param overwrite_amount integer (Default: -1)
function BufModel:TriggerBuf(victim, attacker, loseStack, loseTurn, timing, sinType, overwrite_amount) end

--- Internally Stores a BattleDuelModel.
---@class DuelModel
DuelModel = {}

--- Checks if internal BattleDuelModel reference is null.
---@return boolean
function DuelModel:IsNull() end

---@param action ActionModel
---@return boolean
function DuelModel:HasAction(action) end

--- Fetches the requested action, either 1 or 2.
---@param action_number DuelActionNumber
---@return ActionModel
function DuelModel:FetchAction(action_number) end

--- Sets the requested action, either 1 or 2.
---@param action_number DuelActionNumber
---@param action_set ActionModel
---@return nil
function DuelModel:SetAction(action_number, action_set) end

---@alias DuelActionNumber
---| 1 Fetch _action1
---| 2 Fetch _action2

--- Fetches the opponent action of the provided action within this Duel.
---@param fromAction ActionModel
---@return ActionModel
function DuelModel:FetchOpponentAction(fromAction) end

--- Internally Stores a BattleActionModel.
---@class ActionModel
ActionModel = {}

--- Checks if internal BattleActionModel reference is null.
---@return boolean
function ActionModel:IsNull() end

---@overload fun(self:ActionModel, unitSin:UnitSinModel, unit:UnitModel, sinAction:SinActionModel):ActionModel
---@param unitSin UnitSinModel
---@param unit UnitModel
---@param sinAction SinActionModel
---@param skill SkillModel? Optional
---@return ActionModel
---@nodiscard
function ActionModel:CreateNew(unitSin, unit, sinAction, skill) end

---* `ActionModel:CreateNew()`: Returns a newly created Action.
---* `ActionModel:CreateNew(canDuel)`: Overwrites the original Action's "canDuel".
---
---@overload fun(self:ActionModel):ActionModel
---@param canDuel boolean?
---@return ActionModel
---@nodiscard
function ActionModel:CreateCopy(canDuel) end

---@return UnitModel
function ActionModel:FetchUnitModel() end

---@return SkillModel
function ActionModel:FetchSkill() end

---@return SinActionModel
function ActionModel:FetchSinAction() end

---@param skillID integer
---@return boolean
function ActionModel:TryChangeSkillByID(skillID) end

---@param skill SkillModel
---@param saveOriginSkill boolean (Default = true)
---@return boolean
function ActionModel:TryChangeSkillByModel(skill, saveOriginSkill) end

--- Internally Stores a vanilla SkillModel.
---@class SkillModel
---@field coinList CoinModel[]
SkillModel = {}

--- Checks if internal SkillModel reference is null.
---@return boolean
function SkillModel:IsNull() end

--- Returns all of this thing's modsas.
---@param fast_no_copy boolean (Default = false)
---@return ModsaInterface[]
function SkillModel:FetchAllModsa(fast_no_copy) end

---@return integer
---@nodiscard
function SkillModel:SkillID() end

---@return integer
---@nodiscard
function SkillModel:SkillTier() end

---@return AttributeType
---@nodiscard
function SkillModel:FetchAttribute() end

---@return AtkType
---@nodiscard
function SkillModel:FetchAtkType() end

---@return CoinModel[]
---@nodiscard
function SkillModel:FetchCoinsUsableInDuel() end

---@return CoinModel[]
---@nodiscard
function SkillModel:FetchAliveCoins() end

---@param action ActionModel
---@param coin CoinModel
---@param timing EventTiming
function SkillModel:ReRollCoin(action, coin, timing) end

---@param action ActionModel
---@param idx integer
---@param timing EventTiming
function SkillModel:CopyCoin(action, idx, timing) end

---@param coin CoinModel
---@param forcibly boolean
function SkillModel:AddCoin(coin, forcibly) end

---@param idx integer
function SkillModel:RemoveCoinModelFromListByIndex(idx) end

--- Internally Stores a vanilla CoinModel.
---@class CoinModel
---@field isActive boolean
---@field isUsableInDuel boolean
---@field isReRolled boolean
---@field isDestroyed boolean
---@field result CoinResultAlias
---@field originCoinIndex integer
---@field currentRealCoinIndex integer
---@field scale integer
---@field operatorType CoinOpTypeAlias
CoinModel = {}

--- Checks if internal CoinModel reference is null.
---@return boolean
function CoinModel:IsNull() end

--- Makes a new CoinModel.
---@param fromCoin CoinModel
---@param as_reroll boolean (Default = false)
---@return CoinModel
---@nodiscard
function CoinModel.CreateFromCoinModel(fromCoin, as_reroll) end

--- Makes a new CoinModel.
---@param fromCoin CoinData
---@param originCoinIndex integer
---@return CoinModel
---@nodiscard
function CoinModel.CreateFromCoinData(fromCoin, originCoinIndex) end

---@return CoinData
---@nodiscard
function CoinModel:FetchData() end

--- Returns all of this thing's modsas.
---@param fast_no_copy boolean (Default = false)
---@return ModsaInterface[]
---@nodiscard
function CoinModel:FetchAllModsa(fast_no_copy) end

---@return integer
function CoinModel:GetGrade() end

--- Internally Stores a vanilla SkillCoinData.
---@class CoinData
---@field actionIndex integer
---@field grade integer
---@field scale integer
---@field operatorType CoinOpTypeAlias
CoinData = {}

---@alias CoinOpTypeAlias
---| 0 ADD
---| 1 SUB
---| 2 MUL
---| 3 NONE

---@alias CoinResultAlias
---| 0 NONE
---| 1 HEAD
---| 2 TAIL

--- Internally Stores a vanilla SinActionModel.
---@class SinActionModel
---@field currentSinList UnitSinModel[] Normal upper-lower UnitSinModel in the dashboard (normally 2).
---@field readySinList UnitSinModel[] List of Upcoming Skills (UnitSinModel).
---@field replacedSinListByErode UnitSinModel[] Covered list of UnitSinModel, in case of corrosion.
---@field currentSelectSin UnitSinModel Selected UnitSinModel.
---@field readySin UnitSinModel Upcoming UnitSinModel.
---@field replacedReadySinByErode UnitSinModel The COVERED Upcoming UnitSinModel, in case of corrosion.
---@field replacedSinByDefenseSkill UnitSinModel The COVERED UnitSinModel under the Defense Skill.
SinActionModel = {}

--- Checks if internal SinActionModel reference is null.
---@return boolean
function SinActionModel:IsNull() end

---@return integer
function SinActionModel:GetSlotIndex() end

---@return UnitModel
function SinActionModel:FetchUnitModel() end

--- Internally Stores a vanilla UnitSinModel.
---@class UnitSinModel
---@field currentAction ActionModel
UnitSinModel = {}

--- Checks if internal UnitSinModel reference is null.
---@return boolean
function UnitSinModel:IsNull() end

--- Internally Stores a vanilla ResistDetail.
---@class ResistDetail
---@field atkResistList ATKResistData[]
---@field attributeResistList AttributeResistData[]
ResistDetail = {}

--- Internally Stores a vanilla ATKResistData.
---@class ATKResistData
---@field atkType AtkType
---@field resistanceValue number
ATKResistData = {}

--- Internally Stores a vanilla AttributeResistData.
---@class AttributeResistData
---@field sinType AttributeType
---@field resistanceValue number
AttributeResistData = {}

--- Internally Stores a vanilla BattleUnitView.
---@class UnitView
UnitView = {}

--- Checks if internal BattleUnitView reference is null.
---@return boolean
function UnitView:IsNull() end

---@return UTransform
---@nodiscard
function UnitView:FetchTransform() end

---@return CharAppearance
---@nodiscard
function UnitView:FetchAppearance() end

--- Internally Stores a vanilla CharacterAppearance.
---@class CharAppearance
CharAppearance = {}

--- Checks if internal CharacterAppearance reference is null.
---@return boolean
function CharAppearance:IsNull() end

---@return UTransform
---@nodiscard
function CharAppearance:FetchTransform() end

--- Internally Stores a Unity Transform object.
---@class UTransform
---@field localEuler Vec3
---@field globalEuler Vec3
---@field localScale Vec3
---@field localPos Vec3
---@field globalPos Vec3
UTransform = {}

--- Checks if internal Transform reference is null.
---@return boolean
function UTransform:IsNull() end

---@return UTransform
function UTransform:FetchParent() end

---@param parent UTransform
---@param worldPositionStays boolean
---@return UTransform
function UTransform:SetParent(parent, worldPositionStays) end

--- Internally Stores a Unity Vector3.
---@class Vec3
---@field x number
---@field y number
---@field z number
Vec3 = {}

---@param x number
---@param y number
---@param z number
---@return Vec3
---@nodiscard
function Vec3.CreateFromFloats(x, y, z) end

---@param vec Vec3
---@return Vec3
---@nodiscard
function Vec3.CreateNormalized(vec) end

---@param a Vec3
---@param b Vec3
---@return Vec3
---@nodiscard
function Vec3.Add(a, b) end

---@param a Vec3
---@param b Vec3
---@return Vec3
---@nodiscard
function Vec3.Sub(a, b) end

---@param a Vec3
---@param scalar number
---@return Vec3
---@nodiscard
function Vec3.Mul(a, scalar) end

--- Unlike every other Vec3 method or operation, this directly modifies the reference you're using.
function Vec3:Normalize() end