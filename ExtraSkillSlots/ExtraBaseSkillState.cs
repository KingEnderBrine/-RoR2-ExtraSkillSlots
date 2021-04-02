using EntityStates;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Runtime.CompilerServices;

namespace ExtraSkillSlots
{
    internal class ExtraBaseSkillState
    {
        private static readonly ConditionalWeakTable<BaseSkillState, ExtraBaseSkillState> instances = new ConditionalWeakTable<BaseSkillState, ExtraBaseSkillState>();

        public ExtraSkillLocator ExtraSkillLocator { get; private set; }
        public ExtraInputBankTest ExtraInputBankTest { get; private set; }

        public static void IsKeyDownAuthorityILHook(ILContext il)
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
            c.Emit(OpCodes.Call, typeof(ExtraBaseSkillState).GetMethod(nameof(IsKeyDown), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static));
            c.Emit(OpCodes.Ret);
        }

        private static bool IsKeyDown(BaseSkillState skillState)
        {
            if (!instances.TryGetValue(skillState, out var extraBaseSkillState))
            {
                instances.Add(skillState, extraBaseSkillState = new ExtraBaseSkillState
                {
                    ExtraSkillLocator = skillState.outer.GetComponent<ExtraSkillLocator>(),
                    ExtraInputBankTest = skillState.outer.GetComponent<ExtraInputBankTest>()
                });
            }

            var extraSkillLocator = extraBaseSkillState.ExtraSkillLocator;
            var extraInputBank = extraBaseSkillState.ExtraInputBankTest;

            if (!extraSkillLocator || !extraInputBank)
            {
                return false;
            }

            return (ExtraSkillSlot)skillState.skillLocator.FindSkillSlot(skillState.activatorSkillSlot) switch
            {
                ExtraSkillSlot.ExtraFirst => extraInputBank.extraSkill1.down,
                ExtraSkillSlot.ExtraSecond => extraInputBank.extraSkill2.down,
                ExtraSkillSlot.ExtraThird => extraInputBank.extraSkill3.down,
                ExtraSkillSlot.ExtraFourth => extraInputBank.extraSkill4.down,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
