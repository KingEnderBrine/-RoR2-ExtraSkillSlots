using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExtraSkillSlots
{
    internal static class LanguageConsts
    {
        public const string EXTRA_SKILL_SLOTS_FIRST_EXTRA_SKILL = nameof(EXTRA_SKILL_SLOTS_FIRST_EXTRA_SKILL);
        public const string EXTRA_SKILL_SLOTS_SECOND_EXTRA_SKILL = nameof(EXTRA_SKILL_SLOTS_SECOND_EXTRA_SKILL);
        public const string EXTRA_SKILL_SLOTS_THIRD_EXTRA_SKILL = nameof(EXTRA_SKILL_SLOTS_THIRD_EXTRA_SKILL);
        public const string EXTRA_SKILL_SLOTS_FOURTH_EXTRA_SKILL = nameof(EXTRA_SKILL_SLOTS_FOURTH_EXTRA_SKILL);

        public static void OnLoadStrings(On.RoR2.Language.orig_LoadStrings orig, Language self)
        {
            orig(self);

            switch (self.name.ToLower())
            {
                case "ru":
                    self.SetStringByToken(EXTRA_SKILL_SLOTS_FIRST_EXTRA_SKILL, "Доп. навык 1");
                    self.SetStringByToken(EXTRA_SKILL_SLOTS_SECOND_EXTRA_SKILL, "Доп. навык 2");
                    self.SetStringByToken(EXTRA_SKILL_SLOTS_THIRD_EXTRA_SKILL, "Доп. навык 3");
                    self.SetStringByToken(EXTRA_SKILL_SLOTS_FOURTH_EXTRA_SKILL, "Доп. навык 4");
                    break;
                default:
                    self.SetStringByToken(EXTRA_SKILL_SLOTS_FIRST_EXTRA_SKILL, "Extra skill 1");
                    self.SetStringByToken(EXTRA_SKILL_SLOTS_SECOND_EXTRA_SKILL, "Extra skill 2");
                    self.SetStringByToken(EXTRA_SKILL_SLOTS_THIRD_EXTRA_SKILL, "Extra skill 3");
                    self.SetStringByToken(EXTRA_SKILL_SLOTS_FOURTH_EXTRA_SKILL, "Extra skill 4");
                    break;
            }
        }
    }
}
