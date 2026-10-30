using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using BepInEx.Configuration;
using RiskOfOptions.Options;

namespace ExtraSkillSlots
{
    public class RiskOfOptionsCompat
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void AddConfig()
        {
            RiskOfOptions.ModSettingsManager.AddOption(new ChoiceOption(ExtraSkillSlotsPlugin.ModifierButtonBehavior), ExtraSkillSlotsPlugin.GUID, ExtraSkillSlotsPlugin.Name);
        }
    }
}
