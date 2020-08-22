using EntityStates;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Networking;

namespace ExtraSkillSlots
{
    internal class ExtraBaseSkillState
    {
        private static readonly PropertyInfo skillLocatorProperty = typeof(EntityState).GetProperty("skillLocator", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly Dictionary<BaseSkillState, ExtraBaseSkillState> instances = new Dictionary<BaseSkillState, ExtraBaseSkillState>();

        public ExtraSkillLocator extraSkillLocator { get; private set; }
        public ExtraInputBankTest extraInputBankTest { get; private set; }

        internal static ExtraBaseSkillState Add(BaseSkillState baseSkillState)
        {
            return instances[baseSkillState] = new ExtraBaseSkillState
            {
                extraSkillLocator = baseSkillState.outer.GetComponent<ExtraSkillLocator>(),
                extraInputBankTest = baseSkillState.outer.GetComponent<ExtraInputBankTest>()
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
                
                var extraSkillLocator = extraBaseSkillState.extraSkillLocator;
                var extraInputBank = extraBaseSkillState.extraInputBankTest;
                

                if (!extraSkillLocator || !extraInputBank)
                {
                    return false;
                }

                var skillSlot = (skillLocatorProperty.GetValue(self) as SkillLocator).FindSkillSlot(self.activatorSkillSlot);
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
