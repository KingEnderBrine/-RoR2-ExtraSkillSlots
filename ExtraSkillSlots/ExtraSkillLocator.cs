using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ExtraSkillSlots
{
    /// <summary>
    /// Just a container for extra skill slots
    /// </summary>
    [RequireComponent(typeof(SkillLocator))]
    public class ExtraSkillLocator : MonoBehaviour
    {
        [FormerlySerializedAs("extraSkill1")]
        public GenericSkill extraFirst;
        [FormerlySerializedAs("extraSkill2")]
        public GenericSkill extraSecond;
        [FormerlySerializedAs("extraSkill3")]
        public GenericSkill extraThird;
        [FormerlySerializedAs("extraSkill4")]
        public GenericSkill extraFourth;

        internal static GenericSkill GetSkillOverrideHook(On.RoR2.SkillLocator.orig_GetSkill orig, SkillLocator self, SkillSlot skillSlot)
        {
            var extraSkillLocator = self.GetComponent<ExtraSkillLocator>();
            if (extraSkillLocator)
            {
                if (skillSlot == ExtraSkillSlot.ExtraFirst)
                {
                    return extraSkillLocator.extraFirst;
                }

                if (skillSlot == ExtraSkillSlot.ExtraSecond)
                {
                    return extraSkillLocator.extraSecond;
                }

                if (skillSlot == ExtraSkillSlot.ExtraThird)
                {
                    return extraSkillLocator.extraThird;
                }

                if (skillSlot == ExtraSkillSlot.ExtraFourth)
                {
                    return extraSkillLocator.extraFourth;
                }
            }

            return orig(self, skillSlot);
        }

        internal static SkillSlot FindSkillSlotOverrideHook(On.RoR2.SkillLocator.orig_FindSkillSlot orig, SkillLocator self, GenericSkill skillComponent)
        {
            var extraSkillLocator = self.GetComponent<ExtraSkillLocator>();

            if (!skillComponent)
            {
                return SkillSlot.None;
            }

            if (extraSkillLocator)
            {
                if (skillComponent == extraSkillLocator.extraFirst)
                {
                    return ExtraSkillSlot.ExtraFirst;
                }
                if (skillComponent == extraSkillLocator.extraSecond)
                {
                    return ExtraSkillSlot.ExtraSecond;
                }
                if (skillComponent == extraSkillLocator.extraThird)
                {
                    return ExtraSkillSlot.ExtraThird;
                }
                if (skillComponent == extraSkillLocator.extraFourth)
                {
                    return ExtraSkillSlot.ExtraFourth;
                }
            }

            return orig(self, skillComponent);
        }

        internal static void ApplyAmmoPackILHook(ILContext il)
        {
            var c = new ILCursor(il);

            c.GotoNext(
                x => x.MatchLdcI4(4),
                x => x.MatchNewarr<GenericSkill>());
            c.Next.OpCode = OpCodes.Ldarg_0;
            c.Next.Operand = null;
            c.GotoNext();

            //Replacing skill array initialization with own
            c.RemoveRange(21);
            c.EmitDelegate<Func<SkillLocator, GenericSkill[]>>((self) =>
            {
                var extraSkillLocator = self.GetComponent<ExtraSkillLocator>();
                if (extraSkillLocator)
                {
                    return new GenericSkill[]
                    {
                        self.primary,
                        self.secondary,
                        self.utility,
                        self.special,
                        extraSkillLocator.extraFirst,
                        extraSkillLocator.extraSecond,
                        extraSkillLocator.extraThird,
                        extraSkillLocator.extraFourth,
                    };
                }
                return new GenericSkill[]
                    {
                        self.primary,
                        self.secondary,
                        self.utility,
                        self.special
                    };
            });
        }
    }
}
