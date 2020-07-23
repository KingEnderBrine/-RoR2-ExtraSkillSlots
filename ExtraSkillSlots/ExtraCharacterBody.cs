using ExtraSkillSlots;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace ExtraSkillSlots
{
    internal static class ExtraCharacterBody
    {
        internal static void RecalculateStatsILHook(ILContext il)
        {
            var c = new ILCursor(il);

            c.GotoNext(
                x => x.MatchLdarg(0),
                x => x.MatchLdcR4(0));

            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc, 60);
            c.EmitDelegate<Action<CharacterBody, float>>((characterBody, scale) =>
            {
                var extraSkillLocator = characterBody.GetComponent<ExtraSkillLocator>();
                if (extraSkillLocator)
                {
                    return;
                }
                if (extraSkillLocator.extraFirst)
                {
                    extraSkillLocator.extraFirst.cooldownScale = scale;
                }
                if (extraSkillLocator.extraSecond)
                {
                    extraSkillLocator.extraSecond.cooldownScale = scale;
                }
                if (extraSkillLocator.extraThird)
                {
                    extraSkillLocator.extraThird.cooldownScale = scale;
                }
                if (extraSkillLocator.extraFourth)
                {
                    extraSkillLocator.extraFourth.cooldownScale = scale;
                }
            });
        }
    }
}
