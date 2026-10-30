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
---@param keyword_s string
---@param stack integer
---@param turn integer
---@param activeRound_list integer[]
---@param bufPacket BufPacket
---@return nil
function ModsaInterface:Buf(targetModel, giver, keyword_s, stack, turn, activeRound_list, bufPacket) end

--- Creates and returns a default BufPacket.
---@return BufPacket
function ModsaInterface:CreateBufPacket() end


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

