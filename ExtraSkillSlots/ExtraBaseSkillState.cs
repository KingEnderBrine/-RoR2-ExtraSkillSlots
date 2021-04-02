using EntityStates;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;

namespace ExtraSkillSlots
{
    internal class ExtraBaseSkillState
    {
        internal static readonly Dictionary<BaseSkillState, ExtraBaseSkillState> instances = new Dictionary<BaseSkillState, ExtraBaseSkillState>();

        public ExtraSkillLocator ExtraSkillLocator { get; private set; }
        public ExtraInputBankTest ExtraInputBankTest { get; private set; }

        internal static ExtraBaseSkillState Add(BaseSkillState baseSkillState)
        {
            return instances[baseSkillState] = new ExtraBaseSkillState
            {
                ExtraSkillLocator = baseSkillState.outer.GetComponent<ExtraSkillLocator>(),
                ExtraInputBankTest = baseSkillState.outer.GetComponent<ExtraInputBankTest>()
            };
        }

        internal static void Remove(BaseSkillState baseSkillState)
        {
            instances.Remove(baseSkillState);
        }

        internal static ExtraBaseSkillState Get(BaseSkillState baseSkillState)
        {
            if (instances.TryGetValue(baseSkillState, out var extraBaseSkillState))
            {
                return extraBaseSkillState;
            }
            return null;
        }

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
            c.EmitDelegate<Func<BaseSkillState, bool>>(self =>
            {
                var extraBaseSkillState = Get(self);
                
                var extraSkillLocator = extraBaseSkillState.ExtraSkillLocator;
                var extraInputBank = extraBaseSkillState.ExtraInputBankTest;
                

                if (!extraSkillLocator || !extraInputBank)
                {
                    return false;
                }

                var skillSlot = self.skillLocator.FindSkillSlot(self.activatorSkillSlot);
                if (skillSlot == ExtraSkillSlot.ExtraFirst)
                {
                    return extraInputBank.extraSkill1.down;
                }
                if (skillSlot == ExtraSkillSlot.ExtraSecond)
                {
                    return extraInputBank.extraSkill2.down;
                }
                if (skillSlot == ExtraSkillSlot.ExtraThird)
                {
                    return extraInputBank.extraSkill3.down;
                }
                if (skillSlot == ExtraSkillSlot.ExtraFourth)
                {
                    return extraInputBank.extraSkill4.down;
                }
                throw new ArgumentOutOfRangeException();
            });
            c.Emit(OpCodes.Ret);
        }
    }
}
