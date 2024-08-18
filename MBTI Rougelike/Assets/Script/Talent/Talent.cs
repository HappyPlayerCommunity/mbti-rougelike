using UnityEngine;

[CreateAssetMenu(fileName = "NewTalent", menuName = "Talent System/Talent")]
public class Talent : ScriptableObject
{
    public string talentName;
    public string description;
    public TalentType type;
    public TalentRarity rarity;
    public StatModifier[] statModifiers;
    public bool isEnchantment;

    public void ApplyTalent(Stats stats)
    {
        foreach (var modifier in statModifiers)
        {
            switch (modifier.statType)
            {
                case StatType.FireDamage:
                    stats.fireDamage += modifier.value;
                    break;
                case StatType.MovementSpeed:
                    stats.movementSpeed += modifier.value;
                    break;
                case StatType.Toughness:
                    stats.toughness += modifier.value;
                    break;
                case StatType.BondMovementSpeed:
                    stats.bondMovementSpeed += modifier.value;
                    break;
                case StatType.AttackRange:
                    stats.attackRange += modifier.value;
                    break;
                case StatType.AttackEnergeCharge:
                    stats.attackEnergeCharge += modifier.value;
                    break;
                case StatType.IceDamage:
                    stats.iceDamage += modifier.value;
                    break;
                case StatType.MaxShield:
                    stats.maxShield += modifier.value;
                    break;
                case StatType.ShieldReset:
                    stats.shieldReset += modifier.value;
                    break;
                case StatType.BuildingDurability:
                    stats.buildingDurability += modifier.value;
                    break;
                case StatType.Knockback:
                    stats.knockback += modifier.value;
                    break;
                case StatType.InjuryEnergeCharge:
                    stats.injuryEnergeCharge += modifier.value;
                    break;
                case StatType.EarthDamage:
                    stats.earthDamage += modifier.value;
                    break;
                case StatType.MaxHealth:
                    stats.maxHealth += modifier.value;
                    break;
                case StatType.HealthRegen:
                    stats.healthRegen += modifier.value;
                    break;
                case StatType.ExperienceMultiplier:
                    stats.experienceMultiplier += modifier.value;
                    break;
                case StatType.AttackSpeed:
                    stats.attackSpeed += modifier.value;
                    break;
                case StatType.PhysicalAttackPower:
                    stats.physicalAttackPower += modifier.value;
                    break;
                case StatType.WindDamage:
                    stats.windDamage += modifier.value;
                    break;
                case StatType.Dodge:
                    stats.dodge += modifier.value;
                    break;
                case StatType.AutoEnergeCharge:
                    stats.autoEnergeCharge += modifier.value;
                    break;
                case StatType.Luck:
                    stats.luck += modifier.value;
                    break;
                case StatType.SpecialCooldown:
                    stats.specialCooldown += modifier.value;
                    break;
                case StatType.AbstractAttackPower:
                    stats.abstractAttackPower += modifier.value;
                    break;
                case StatType.ThunderDamage:
                    stats.thunderDamage += modifier.value;
                    break;
                case StatType.GlobalAttackPower:
                    stats.globalAttackPower += modifier.value;
                    break;
                case StatType.WaterDamage:
                    stats.waterDamage += modifier.value;
                    break;
                case StatType.HealingPower:
                    stats.healingPower += modifier.value;
                    break;
                case StatType.Crit:
                    stats.crit += modifier.value;
                    break;
                case StatType.CritDamage:
                    stats.critDamage += modifier.value;
                    break;
                case StatType.BuildingPower:
                    stats.buildingPower += modifier.value;
                    break;
                case StatType.BondPower:
                    stats.bondPower += modifier.value;
                    break;
                case StatType.StatusPower:
                    stats.statusPower += modifier.value;
                    break;
                case StatType.StatusDuration:
                    stats.statusDuration += modifier.value;
                    break;
                case StatType.StatusImpact:
                    stats.statusImpact += modifier.value;
                    break;
                case StatType.AnomalyResistance:
                    stats.anomalyResistance += modifier.value;
                    break;
            }

        }
    }

    public void RemoveTalent(Stats stats)
    {
        foreach (var modifier in statModifiers)
        {
            switch (modifier.statType)
            {
                case StatType.FireDamage:
                    stats.fireDamage -= modifier.value;
                    break;
                case StatType.MovementSpeed:
                    stats.movementSpeed -= modifier.value;
                    break;
                case StatType.Toughness:
                    stats.toughness -= modifier.value;
                    break;
                case StatType.BondMovementSpeed:
                    stats.bondMovementSpeed -= modifier.value;
                    break;
                case StatType.AttackRange:
                    stats.attackRange -= modifier.value;
                    break;
                case StatType.AttackEnergeCharge:
                    stats.attackEnergeCharge -= modifier.value;
                    break;
                case StatType.IceDamage:
                    stats.iceDamage -= modifier.value;
                    break;
                case StatType.MaxShield:
                    stats.maxShield -= modifier.value;
                    break;
                case StatType.ShieldReset:
                    stats.shieldReset -= modifier.value;
                    break;
                case StatType.BuildingDurability:
                    stats.buildingDurability -= modifier.value;
                    break;
                case StatType.Knockback:
                    stats.knockback -= modifier.value;
                    break;
                case StatType.InjuryEnergeCharge:
                    stats.injuryEnergeCharge -= modifier.value;
                    break;
                case StatType.EarthDamage:
                    stats.earthDamage -= modifier.value;
                    break;
                case StatType.MaxHealth:
                    stats.maxHealth -= modifier.value;
                    break;
                case StatType.HealthRegen:
                    stats.healthRegen -= modifier.value;
                    break;
                case StatType.ExperienceMultiplier:
                    stats.experienceMultiplier -= modifier.value;
                    break;
                case StatType.AttackSpeed:
                    stats.attackSpeed -= modifier.value;
                    break;
                case StatType.PhysicalAttackPower:
                    stats.physicalAttackPower -= modifier.value;
                    break;
                case StatType.WindDamage:
                    stats.windDamage -= modifier.value;
                    break;
                case StatType.Dodge:
                    stats.dodge -= modifier.value;
                    break;
                case StatType.AutoEnergeCharge:
                    stats.autoEnergeCharge -= modifier.value;
                    break;
                case StatType.Luck:
                    stats.luck -= modifier.value;
                    break;
                case StatType.SpecialCooldown:
                    stats.specialCooldown -= modifier.value;
                    break;
                case StatType.AbstractAttackPower:
                    stats.abstractAttackPower -= modifier.value;
                    break;
                case StatType.ThunderDamage:
                    stats.thunderDamage -= modifier.value;
                    break;
                case StatType.GlobalAttackPower:
                    stats.globalAttackPower -= modifier.value;
                    break;
                case StatType.WaterDamage:
                    stats.waterDamage -= modifier.value;
                    break;
                case StatType.HealingPower:
                    stats.healingPower -= modifier.value;
                    break;
                case StatType.Crit:
                    stats.crit -= modifier.value;
                    break;
                case StatType.CritDamage:
                    stats.critDamage -= modifier.value;
                    break;
                case StatType.BuildingPower:
                    stats.buildingPower -= modifier.value;
                    break;
                case StatType.BondPower:
                    stats.bondPower -= modifier.value;
                    break;
                case StatType.StatusPower:
                    stats.statusPower -= modifier.value;
                    break;
                case StatType.StatusDuration:
                    stats.statusDuration -= modifier.value;
                    break;
                case StatType.StatusImpact:
                    stats.statusImpact -= modifier.value;
                    break;
                case StatType.AnomalyResistance:
                    stats.anomalyResistance -= modifier.value;
                    break;
            }

        }
    }
}

public enum StatType
{
    FireDamage,
    MovementSpeed,
    Toughness,
    BondMovementSpeed,
    AttackRange,
    AttackEnergeCharge,
    IceDamage,
    MaxShield,
    ShieldReset,
    BuildingDurability,
    Knockback,
    InjuryEnergeCharge,
    EarthDamage,
    MaxHealth,
    HealthRegen,
    ExperienceMultiplier,
    AttackSpeed,
    PhysicalAttackPower,
    WindDamage,
    Dodge,
    AutoEnergeCharge,
    Luck,
    SpecialCooldown,
    AbstractAttackPower,
    ThunderDamage,
    GlobalAttackPower,
    WaterDamage,
    HealingPower,
    Crit,
    CritDamage,
    BuildingPower,
    BondPower,
    StatusPower,
    StatusDuration,
    StatusImpact,
    AnomalyResistance,
}

public enum TalentType
{
    E,
    I,
    S,
    N,
    T,
    F,
    J,
    P
}

[System.Serializable]
public struct StatModifier
{
    public StatType statType;
    public float value;
}

public enum TalentRarity
{
    Basic = 0,     // 基础天赋
    Advanced = 1,  // 进阶天赋
    Elite = 2,     // 高阶天赋
    Count = 3,
}