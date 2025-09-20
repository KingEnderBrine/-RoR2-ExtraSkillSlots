using Rewired;
using RoR2;

namespace ExtraSkillSlots
{
    public class RewiredAction
    {
        public static RewiredAction FirstExtraSkill { get; }
        public static RewiredAction SecondExtraSkill { get; }
        public static RewiredAction ThirdExtraSkill { get; }
        public static RewiredAction FourthExtraSkill { get; }

        public int ActionId { get; private set; }
        public string Name { get; private set; }
        public string DisplayToken { get; private set; }
        public KeyboardKeyCode DefaultKeyboardKey { get; private set; }
        public int DefaultJoystickKey { get; private set; }

        private InputAction inputAction;

        private ActionElementMap _defaultJoystickMap;
        public ActionElementMap DefaultJoystickMap { get => _defaultJoystickMap ??= new ActionElementMap(ActionId, ControllerElementType.Button, DefaultJoystickKey, Pole.Positive, AxisRange.Positive); }

        private ActionElementMap _defaultKeyboardMap;
        public ActionElementMap DefaultKeyboardMap { get => _defaultKeyboardMap ??= new ActionElementMap(ActionId, ControllerElementType.Button, (int)DefaultKeyboardKey - 21) { _keyboardKeyCode = DefaultKeyboardKey }; }


        static RewiredAction()
        {
            FirstExtraSkill = new RewiredAction
            {
                ActionId = 100,
                Name = "FirstExtraSkill",
                DisplayToken = LanguageConsts.EXTRA_SKILL_SLOTS_FIRST_EXTRA_SKILL,
                DefaultKeyboardKey = KeyboardKeyCode.Alpha1,
                DefaultJoystickKey = 16
            };

            SecondExtraSkill = new RewiredAction
            {
                ActionId = 101,
                Name = "SecondExtraSkill",
                DisplayToken = LanguageConsts.EXTRA_SKILL_SLOTS_SECOND_EXTRA_SKILL,
                DefaultKeyboardKey = KeyboardKeyCode.Alpha2,
                DefaultJoystickKey = 17
            };

            ThirdExtraSkill = new RewiredAction
            {
                ActionId = 102,
                Name = "ThirdExtraSkill",
                DisplayToken = LanguageConsts.EXTRA_SKILL_SLOTS_THIRD_EXTRA_SKILL,
                DefaultKeyboardKey = KeyboardKeyCode.Alpha3,
                DefaultJoystickKey = 18
            };

            FourthExtraSkill = new RewiredAction
            {
                ActionId = 103,
                Name = "FourthExtraSkill",
                DisplayToken = LanguageConsts.EXTRA_SKILL_SLOTS_FOURTH_EXTRA_SKILL,
                DefaultKeyboardKey = KeyboardKeyCode.Alpha4,
                DefaultJoystickKey = 19
            };
        }

        public static implicit operator InputCatalog.ActionAxisPair(RewiredAction action)
        {
            return new InputCatalog.ActionAxisPair(action.Name, AxisRange.Full);
        }

        public static implicit operator InputAction(RewiredAction action)
        {
            return action.inputAction ??= new InputAction
            {
                id = action.ActionId,
                name = action.Name,
                type = InputActionType.Button,
                descriptiveName = action.Name,
                behaviorId = 0,
                userAssignable = true,
                categoryId = 0
            };
        }
    }
}
