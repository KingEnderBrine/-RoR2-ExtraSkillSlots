using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;

namespace ExtraSkillSlots
{
    internal static class ExtraCharacterBody
    {
        internal static void RecalculateStatsILHook(ILContext il)
        {
            var c = new ILCursor(il);

            var cooldownScaleIndex = -1;
            var flatCooldownReductionIndex = -1;

            c.GotoNext(
                x => x.MatchLdfld<SkillLocator>(nameof(SkillLocator.primary)));
            c.GotoNext(
                x => x.MatchLdloc(out cooldownScaleIndex),
                x => x.MatchCallOrCallvirt<GenericSkill>("set_cooldownScale"));
            c.GotoNext(
                x => x.MatchLdloc(out flatCooldownReductionIndex),
                x => x.MatchCallOrCallvirt<GenericSkill>("set_flatCooldownReduction"));

            c.GotoNext(
                MoveType.After,
                x => x.MatchLdarg(0),
                x => x.MatchLdcR4(0),
                x => x.MatchCallOrCallvirt<CharacterBody>("set_critHeal"));
            
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc, cooldownScaleIndex);
            c.Emit(OpCodes.Ldloc, flatCooldownReductionIndex);
            c.Emit(OpCodes.Call, typeof(ExtraCharacterBody).GetMethod(nameof(RecalculateCooldowns), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static));
        }

        private static void RecalculateCooldowns(CharacterBody characterBody, float scale, float flatCooldownReduction)
        {
            var extraSkillLocator = characterBody.GetComponent<ExtraSkillLocator>();
            if (!extraSkillLocator)
            {
                return;
            }
            if (extraSkillLocator.extraFirst)
            {
                extraSkillLocator.extraFirst.cooldownScale = scale;
                extraSkillLocator.extraFirst.flatCooldownReduction = flatCooldownReduction;
            }
            if (extraSkillLocator.extraSecond)
            {
                extraSkillLocator.extraSecond.cooldownScale = scale;
                extraSkillLocator.extraSecond.flatCooldownReduction = flatCooldownReduction;
            }
            if (extraSkillLocator.extraThird)
            {
                extraSkillLocator.extraThird.cooldownScale = scale;
                extraSkillLocator.extraThird.flatCooldownReduction = flatCooldownReduction;
            }
            if (extraSkillLocator.extraFourth)
            {
                extraSkillLocator.extraFourth.cooldownScale = scale;
                extraSkillLocator.extraFourth.flatCooldownReduction = flatCooldownReduction;
            }
        }
    }
}
