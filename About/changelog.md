

# ChangeLog


##ToDo - General
----------------------------------------------------------------------------
* EquipComp - look into
* review the active effect - https://github.com/jecrell/CompActivatableEffect


##ToDo - Wizard
----------------------------------------------------------------------------

### Importance: Next Release
-------------------------




* Druid
    * artwork
      * Entangling Vines
      * Popping fern
      * Druid Tree
    * sickening Vines
  * fix plants so they don't die after maturity
* Wizard
    * ring recipe to convert between fingers
    * wizard comms console (smaller)
    * sell price for wizard items so they can earn a living Wizardry-ing
    * fix temperature devices to use Runes (and rename)
    * Research: Descriptions about what is in each category.
    * video/gifs about this
    * items
      * hats
      * held items
      * magic items
      * boots of speed
      * imbue items with power ("of speed:MoveSpeed","of Skill:GlobalWorkSpeed:WorkTableWorkSpeedFactor","of Endurance:MaxHitPoints","of Quality:DeteriorationRate","of Learning:GlobalLearningFactor","of Comfort:Comfort", "of Calm:MentalBreakThreshold", "of Brawling:MeleeHitChance:MeleeWeapon_DamageAmount:MeleeWeapon_Cooldown:SharpDamageMultiplier:BluntDamageMultiplier", "of the Mole:MiningSpeed", "of Study:ResearchSpeedFactor:ResearchSpeed", "of BUilding:ConstructionSpeed:ConstructFailChance:FixBrokenDownBuildingFailChance", "of Healing:HealingSpeed:HealingQuality:ImmunityGainSpeed:BedRestEffectiveness", "of Charaisma:TradePriceImprovement:Haggling:SocialImpact", "of Gardening:PlantWorkSpeed:HarvestFailChance", "of Animalism:TameAnimalChance:TrainAnimalChance", "of Accuracy:ShootingAccuracy:AimingDelayFactor:RangedWeapon_Cooldown:Accuracy[Touch|Short|Medium|Long]","
    * wand assortment:
        * fireball
        * ice
    * recipes
        * rings to transform between hands
    * Artwork
        * rings
        * glow on zap

### Importance: High
-------------------------
    * hauling wisp (less artwork than imp)
    * wizard skill is 0, but on gain WizardTrait, set to 1
        * recipes require skill level at least 1
    * figure out wand placement in skill hierarchy
    * wand:
        * crippling pain
        * ignite pawn only (not ground)
        * Plasma (DOT)
    * Artwork
        * robes
    * Gizmos
        * figure out how to add and use/respond to them

### Importance: Med
-------------------------
    * generic projectile Comp for all classes
    * ArcaneFloors power research and artificer and pylons and walls
    * Sound
        * wand zaps

### Importance: Low
-------------------------
    * window popups on research events
    * new category for Wizard things in menu (so they don't get mixed?)
    * Devilstrand in Vats
    * AI change to want to carry multiple wands
    * change RecipeMakerProperties so there is also a trait requirement so
     only Wizards can make certain recipes
     * somethig like public List<TraitDef> traitPrerequisites;

### Importance: Skipping
-------------------------
    * breakdown event for arcane consumes 1 script and not components



## ToDo - Druid
============================================================================
* trap that explodes in twisting vines, to slow people. can be reset and
  vines can be cleaned up (use Rubble ThingDef) or rot


# v.4 -
============================================================================
* General
  * Created UnificaMagica category for most every item in this Mod to go in

* Druid
  * adjust temperature marking
     * made components and properties to change plant livable temperatures
  * created druid tree
* Wizard
  * made new work type based upon wizardry and not smithing... tho doesnt change anything
  * put skill restrictions on runes
  * reduce research times for skills
  * made vats movable
  * fixed HeavyArcer recipe
  * removed craftable items from Machine Table
  * fix temperature item components
  * get rid of scripts


# v.3 - Wizard Runes replace Scripts; Hello Druid
============================================================================
* not well tested since I want to get this out.
* created IExtendedThing so modding will be easier
* an arcane deviltrand plant for ArcaneGrowVats
* added recipes for mortar shells and mechanoid butchering to ArcaneWorkBenches
* wand assortment:
  * channeled bursts (chain gun)
  * disintegrate
  * crippling (short term nausea)
* druid
  * defensive plot to grow plants in
  * entangling vine plant
  * popping fern plant
* Runes instead of Scripts for wizard resource
  * more variety and thinking about what you're making
  * some runes harder to make
  * rebalanced resources to make things so the relative weights are better
  * Types
    * power, binding, pulsing, control, awareness
    * Old Magic - harder to make


# v.2 - Wizard Improvements and More Playable
============================================================================
* Wand assortment: (class to handle easily creating projectiles now)
    * zap, zap powerful
    * lightning
    * explosive
* Artwork
    * make blank turret top for arcers and pilons
    * darken scripts on doors and walls, more purple
* craftable wizard shield
* wand zaps have a flash, LIGHTNGING
* Trait: Wizard Inclined - If you go down the path of Wizardry, you'll gain wizard
    powers but it will cost you. Eat the Wizard Pill (craft it) and you can
    start using wizard items and benches. Careful, being a wizard comes at a
    price of very slow movement.
* added larger wizard lamps and made small wizard lamp look smaller
* changes small wizard lamp color to be whiter
* inject screen showing skills
* Grow vats are faster and consume more scripts





# v.1 - Initial Creation of Wizard
============================================================================
* released with basic stuff
