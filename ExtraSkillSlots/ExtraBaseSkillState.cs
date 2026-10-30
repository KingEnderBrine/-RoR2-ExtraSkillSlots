using EntityStates;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2BepInExPack.Utilities;
using System;

namespace ExtraSkillSlots
{
    internal class ExtraBaseSkillState
    {
        private static readonly FixedConditionalWeakTable<ISkillState, ExtraBaseSkillState> instances = new FixedConditionalWeakTable<ISkillState, ExtraBaseSkillState>();

        public ExtraInputBankTest ExtraInputBankTest { get; private set; }
        public ExtraSkillLocator ExtraSkillLocator { get; private set; }

        public static void IsKeyDownAuthorityILHook(ILContext il)
        {
            SkillMethodIl(il, IsKeyDown);
        }

        public static void IsKeyJustPressedAuthorityILHook(ILContext il)
        {
            SkillMethodIl(il, IsKeyJustPressed);
        }

        public static void IsKeyJustReleasedAuthorityILHook(ILContext il)
        {
            SkillMethodIl(il, IsKeyJustReleased);
        }

        public static void SetKeyPressClaimedAuthorityILHook(ILContext il)
        {
            SkillMethodIl(il, SetKeyPressClaimed);
        }

        public static void IsKeyPressClaimedAuthorityILHook(ILContext il)
        {
            SkillMethodIl(il, IsKeyPressClaimed);
        }

        public static void GetSkillDamageSourceIlHook(ILContext il)
        {
            var c = new ILCursor(il);

            c.GotoNext(
                x => x.MatchNewobj<ArgumentOutOfRangeException>(),
                x => x.MatchThrow());
            c.Index++;
            c.Previous.OpCode = OpCodes.Nop;
            c.Previous.Operand = null;

            c.Remove();
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate(GetSkillDamageSource);
            c.Emit(OpCodes.Ret);
        }

        private static void SkillMethodIl(ILContext il, Func<ISkillState, SkillLocator, bool> func)
        {
            var c = new ILCursor(il);

            c.GotoNext(
                x => x.MatchNewobj<ArgumentOutOfRangeException>(),
                x => x.MatchThrow());
            c.Index++;
            c.Previous.OpCode = OpCodes.Nop;
            c.Previous.Operand = null;

            c.Remove();
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate(func);
            c.Emit(OpCodes.Ret);
        }

        private static bool IsKeyDown(ISkillState skillState, SkillLocator skillLocator)
        {
            if (!TryGetExtraInputBank(skillState, skillLocator, out var extraInputBank, out _))
            {
                return false;
            }

            return (ExtraSkillSlot)skillLocator.FindSkillSlot(skillState.activatorSkillSlot) switch
            {
                ExtraSkillSlot.ExtraFirst => extraInputBank.extraSkill1.down,
                ExtraSkillSlot.ExtraSecond => extraInputBank.extraSkill2.down,
                ExtraSkillSlot.ExtraThird => extraInputBank.extraSkill3.down,
                ExtraSkillSlot.ExtraFourth => extraInputBank.extraSkill4.down,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static bool IsKeyJustPressed(ISkillState skillState, SkillLocator skillLocator)
        {
            if (!TryGetExtraInputBank(skillState, skillLocator, out var extraInputBank, out _))
            {
                return false;
            }

            return (ExtraSkillSlot)skillLocator.FindSkillSlot(skillState.activatorSkillSlot) switch
            {
                ExtraSkillSlot.ExtraFirst => extraInputBank.extraSkill1.justPressed,
                ExtraSkillSlot.ExtraSecond => extraInputBank.extraSkill2.justPressed,
                ExtraSkillSlot.ExtraThird => extraInputBank.extraSkill3.justPressed,
                ExtraSkillSlot.ExtraFourth => extraInputBank.extraSkill4.justPressed,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static bool IsKeyJustReleased(ISkillState skillState, SkillLocator skillLocator)
        {
            if (!TryGetExtraInputBank(skillState, skillLocator, out var extraInputBank, out _))
            {
                return false;
            }

            return (ExtraSkillSlot)skillLocator.FindSkillSlot(skillState.activatorSkillSlot) switch
            {
                ExtraSkillSlot.ExtraFirst => extraInputBank.extraSkill1.justReleased,
                ExtraSkillSlot.ExtraSecond => extraInputBank.extraSkill2.justReleased,
                ExtraSkillSlot.ExtraThird => extraInputBank.extraSkill3.justReleased,
                ExtraSkillSlot.ExtraFourth => extraInputBank.extraSkill4.justReleased,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static bool SetKeyPressClaimed(ISkillState skillState, SkillLocator skillLocator)
        {
            if (!TryGetExtraInputBank(skillState, skillLocator, out var extraInputBank, out _))
            {
                return false;
            }

            return (ExtraSkillSlot)skillLocator.FindSkillSlot(skillState.activatorSkillSlot) switch
            {
                ExtraSkillSlot.ExtraFirst => extraInputBank.extraSkill1.hasPressBeenClaimed = true,
                ExtraSkillSlot.ExtraSecond => extraInputBank.extraSkill2.hasPressBeenClaimed = true,
                ExtraSkillSlot.ExtraThird => extraInputBank.extraSkill3.hasPressBeenClaimed = true,
                ExtraSkillSlot.ExtraFourth => extraInputBank.extraSkill4.hasPressBeenClaimed = true,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static bool IsKeyPressClaimed(ISkillState skillState, SkillLocator skillLocator)
        {
            if (!TryGetExtraInputBank(skillState, skillLocator, out var extraInputBank, out _))
            {
                return false;
            }

            return (ExtraSkillSlot)skillLocator.FindSkillSlot(skillState.activatorSkillSlot) switch
            {
                ExtraSkillSlot.ExtraFirst => extraInputBank.extraSkill1.hasPressBeenClaimed,
                ExtraSkillSlot.ExtraSecond => extraInputBank.extraSkill2.hasPressBeenClaimed,
                ExtraSkillSlot.ExtraThird => extraInputBank.extraSkill3.hasPressBeenClaimed,
                ExtraSkillSlot.ExtraFourth => extraInputBank.extraSkill4.hasPressBeenClaimed,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static DamageSource GetSkillDamageSource(ISkillState skillState, SkillLocator skillLocator)
        {
            if (!TryGetExtraInputBank(skillState, skillLocator, out _, out var extraSkillLocator))
            {
                return DamageSource.NoneSpecified;
            }

            return (ExtraSkillSlot)skillLocator.FindSkillSlot(skillState.activatorSkillSlot) switch
            {
                ExtraSkillSlot.ExtraFirst => extraSkillLocator.extraFirstDamageSource,
                ExtraSkillSlot.ExtraSecond => extraSkillLocator.extraSecondDamageSource,
                ExtraSkillSlot.ExtraThird => extraSkillLocator.extraThirdDamageSource,
                ExtraSkillSlot.ExtraFourth => extraSkillLocator.extraFourthDamageSource,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static bool TryGetExtraInputBank(ISkillState skillState, SkillLocator skillLocator, out ExtraInputBankTest extraInputBank, out ExtraSkillLocator extraSkillLocator)
        {
            if (!instances.TryGetValue(skillState, out var extraBaseSkillState))
            {
                instances.Add(skillState, extraBaseSkillState = new ExtraBaseSkillState
                {
                    ExtraInputBankTest = skillLocator.GetComponent<ExtraInputBankTest>(),
                    ExtraSkillLocator = skillLocator.GetComponent<ExtraSkillLocator>(),
                });
            }

            extraInputBank = extraBaseSkillState.ExtraInputBankTest;
            extraSkillLocator = extraBaseSkillState.ExtraSkillLocator;
            return extraInputBank && extraSkillLocator;
        }
    }
}
