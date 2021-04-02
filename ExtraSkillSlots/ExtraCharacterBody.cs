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

            c.GotoNext(
                x => x.MatchLdarg(0),
                x => x.MatchLdcR4(0),
                x => x.MatchCallOrCallvirt<CharacterBody>("set_critHeal"));
            c.Index++;
            
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc, 66);
            c.Emit(OpCodes.Ldloc, 63);
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
