# For users
It's utility mod and does almost nothing on its own.
What you should know about it is that mod has key bindings for extra skills that must be assigned in settings.
Here's, I think, the best key bindings (default, starting from 1.4.0):
![Keybingins](https://github.com/KingEnderBrine/-RoR2-ExtraSkillSlots/blob/master/Screenshots/1.jpg?raw=true)

# For developers
#### TL; DR;
Adding extra skill is almost the same as adding normal skills.
Along with `SkillLocator`, you need to use `ExtraSkillLocator`. It contains info about extra skills. 
Here's code snipet for adding extra skills:
```
var extraSkillLocator = commandoPrefab.AddComponent<ExtraSkillLocator>();

extraSkillLocator.extraFirst = firstExtraSkill;
extraSkillLocator.extraSecond = secondExtraSkill;
extraSkillLocator.extraThird = thirdExtraSkill;
extraSkillLocator.extraFourth = fourthExtraSkill;
```
That's all it takes for adding simple skills.

#### Full documentation
All defined classes extend corresponding classes from RoR2. Class naming follows one pattern: `Extra{NameOfRoR2Class}.`

* `ExtraSkillLocator` - The main thing that you need. It's a component that you add to your character prefab to be able to use extra skills.
**Fields**:
    * GenericSkill `extraFirst` - contains info about first extra skill
    * GenericSkill `extraSecond` - contains info about second extra skill
    * GenericSkill `extraThird` - contains info about third extra skill
    * GenericSkill `extraFourth` - contains info about fourth extra skill

* `ExtraInputBankTest` - Containts info about user inputs state for extra skills.
**Fields**:
    * InputBankTest.ButtonState `extraSkill1` - contains info about first extra skill state
    * InputBankTest.ButtonState `extraSkill2` - contains info about second extra skill state
    * InputBankTest.ButtonState `extraSkill3` - contains info about third extra skill state
    * InputBankTest.ButtonState `extraSkill4` - contains info about fourth extra skill state

* `ExtraSkillSlot` - Contains consts for extra skill slots. Can be used to get skills from the corresponding slot from `SkillLocator`.

* `RewiredActions` - Contains consts for rewired actions. With that info, you can manually access corresponding buttons state via `Rewired.Player`

* `ExtraSkillSlotsPlugin` - Main class of this mod. There's nothing you can do with it, except to look at BepInPlugin definition to define a dependency to that mod.
***
# Small tips
If you got to the point where you need more skills, you probably know enough about creating characters/skills, just want to leave here a small guide for skills creating.

* To access `ExtraInputBankTest`(and any component that not stored in fields) from `EntityState` you should use `outer` field, for example:
```
var extraInputBankTest = outer.GetComponent<ExtraInputBankTest>();
```

* Mini guide about adding extra skill
```
//Create new SkillFamily
var firstSkillFamily = ScriptableObject.CreateInstance<SkillFamily>();

//IMPORTANT! Do not forget to add name for SkillFamily 
//because game uses it for saving loadout
//Also I recomend to follow naming convention "{PrefabName}{SkillSlot}Family";
(firstSkillFamily as ScriptableObject).name = "MyCharacterBodyFirstExtraFamily";

//Adding skill variants to the family
firstSkillFamily.variants = variants;

//Registering skill family in catalog
LoadoutAPI.AddSkillFamily(firstSkillFamily);

//Adding new GenericSkill component to character prefab
var firstExtraSkill = myCharacterPrefab.AddComponent<GenericSkill>();

//Setting SkillFamily to our GenericSkill
firstExtraSkill.SetFieldValue("_skillFamily", firstSkillFamily);

...

//Adding ExtraSkillLocator to character prefab
var extraSkillLocator = myCharacterPrefab.AddComponent<ExtraSkillLocator>();

//Assigning our skill to corresponding slot
extraSkillLocator.extraFirst = firstExtraSkill;
```

# Bugs
Feel free to ping me on discord `@KingEnderBrine` if you found one.
