using EntityStates;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2BepInExPack.Utilities;
using System;

namespace ExtraSkillSlots
{
    internal class ExtraBaseSkillState
    {
        private static readonly FixedConditionalWeakTable<BaseSkillState, ExtraBaseSkillState> instances = new FixedConditionalWeakTable<BaseSkillState, ExtraBaseSkillState>();

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
            c.EmitDelegate(IsKeyDown);
            c.Emit(OpCodes.Ret);
        }

        private static bool IsKeyDown(BaseSkillState skillState)
        {
            if (!instances.TryGetValue(skillState, out var extraBaseSkillState))
            {
                instances.Add(skillState, extraBaseSkillState = new ExtraBaseSkillState
                {
                    ExtraInputBankTest = skillState.outer.GetComponent<ExtraInputBankTest>()
                });
            }

            var extraInputBank = extraBaseSkillState.ExtraInputBankTest;

            if (!extraInputBank)
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
