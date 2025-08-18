extends Resource
class_name RoleData
@export var Tag: String

@export_group("WeaponSlots")
@export var WeaponSlot0: Array[GDWeaponKinds.Kind]
@export var WeaponSlot1: Array[GDWeaponKinds.Kind]
@export var WeaponSlot2: Array[GDWeaponKinds.Kind]

@export_group("GenericGrowths")
#Steigung und Y-Achsenabschnitt für eine lineare Funktion
#Diese Funktion generiert Werte pro Level
@export var BaseDefenseSlopeBase: Array[float]
@export var SkillSlopeBase: Array[float]
@export var ReactionSlopeBase: Array[float]
@export var FocusSlopeBase: Array[float]
@export var AuraSlopeBase: Array[float]
@export var PrecisionSlopeBase: Array[float]
