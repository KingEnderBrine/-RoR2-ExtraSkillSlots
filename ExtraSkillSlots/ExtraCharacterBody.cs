using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;

namespace ExtraSkillSlots
{
    [HarmonyPatch]
    internal static class ExtraCharacterBody
    {
        [HarmonyILManipulator, HarmonyPatch(typeof(CharacterBody),nameof(CharacterBody.RecalculateStats))]
        internal static void RecalculateStatsILHook(ILContext il)
        {
            var c = new ILCursor(il);

            var cooldownScaleIndex = -1;
            var flatCooldownReductionIndex = -1;

            c.GotoNext(x => x.MatchLdfld<SkillLocator>(nameof(SkillLocator.primary)));
            
            c.GotoNext(x => x.MatchCallOrCallvirt<GenericSkill>("set_cooldownScale"));
            c.GotoPrev(x => x.MatchLdloc(out cooldownScaleIndex));
            
            c.GotoNext(x => x.MatchCallOrCallvirt<GenericSkill>("set_flatCooldownReduction"));
            c.GotoPrev(x => x.MatchLdloc(out flatCooldownReductionIndex));

            c.GotoNext(MoveType.Before, x => x.MatchRet());
            
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
