using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using AbilityUser;

namespace UnificaMagica
{

    public class CompAbilityUserWizard : CompAbilityUser
    {
        public string LabelKey = "UM_Wizard";
/*
        public int ii = 1;
        public int jj;

        public CompAbilityUserWizard() {
            Log.Message("CompAblityUserWizard");
        }
        */
        //public AbilityDef wizardbolt = UnificaMagicaDefOf.WizardBoltAbilityDef;// AbilityDefOf.WizardBoltDef;

        public override void CompTick() {
            base.CompTick();
            if ( IsInitialized ) {
            }
        }

        public int WizardryLevel {
            get {
                return this.abilityUser.skills.GetSkill(UnificaMagicaDefOf.Wizardry).Level;
            }
        }

        public int NumSelectedInLevel(int lvl) {
            int retval = 0;
            foreach ( PawnAbility pa in this.Powers ) {
                UMAbilityDef wad = pa.powerdef as UMAbilityDef;
                if ( wad != null && wad.abilityLevel == lvl ) { retval ++; }
            }
            return retval;
        }

        //Level	1	2	3
        // 1	1	0	-1
        //2	1	0	-1
        //3	1	0	-1
        //4	1	0	0
        //5	1	1	0
        //6	2	1	0
        //7	2	1	0
        //8	2	1	0
        //9	2	1	0
        //10	2	1	0
        //11	2	1	0
        //12	3	2	1
        //13	3	2	1
        //14	3	2	1
        //15	3	2	1
        //16	3	2	1
        //17	3	2	1
        //18	4	2	1
        //19	4	3	1
        //20	4	3	2
        public int NumAvailableAtLevel(int lvl) {
            int retval = 0;
            int offset;
            double divby;
            switch(lvl) {
                case 1: offset = -1; divby = 6; break;
                case 2: offset =  4; divby = 7; break;
                case 3: offset = 11; divby = 8; break;
                default:
                Log.Warning("Unknown level of "+lvl+" for Wizard NumAvailablePerLevel");
                return 0;
            };
            retval = (int) Math.Ceiling( (double) ( ((double)(this.WizardryLevel-offset)) / divby) );
            if ( retval < 0 ) retval = 0;
            return retval;
        }

        public int GetNumOfAbility(UMAbilityDef ab) {
            int retval = 0;
            foreach (PawnAbility pa in this.Powers ) { if ( pa.powerdef == ab ) retval++; }
            return retval;
        }


        // Ordered by level, list of all abilities
        public static List<UMAbilityDef> WizardAbilities= null;
        // Ordered by level, list of all abilities, of a certain level
        protected static List<UMAbilityDef>[] wizardAbilitiesByLevel = null;
        public static int MaxWizardSpellLevel = 3;

        public static List<UMAbilityDef> GetWizardAbilitiesOfLevel(int lvl) {
            List<UMAbilityDef> retval = null;
            if ( lvl <= MaxWizardSpellLevel && lvl > 0 ) retval = wizardAbilitiesByLevel[lvl-1];
            else Log.Warning("WizardAbilitiesOfLevel error; no lvl of "+lvl+". Returning null.");
            return retval;
        }

        public bool TryAddPawnAbility(UMAbilityDef ability) {
            bool retval = false;

            int curselectedinlevel = this.NumAvailableAtLevel(ability.abilityLevel);
//            Log.Message("TryAddPawnAbilty "+curselectedinlevel+" " + this.NumAvailableAtLevel(ability.abilityLevel));
            if ( curselectedinlevel < this.NumAvailableAtLevel(ability.abilityLevel) ) {
                this.AddPawnAbility(ability);
                retval = true;
            }
            return retval;
        }
        //this.AddPawnAbility(UnificaMagicaDefOf.UM_WizardBolt);
        //this.AddPawnAbility(UnificaMagicaDefOf.UM_FearPerson);
        //this.AddPawnAbility(UnificaMagicaDefOf.UM_FearBomb);

        // adds spells to wizard at initilization
        protected static void InitWizardAbility(UMAbilityDef spell) {
            CompAbilityUserWizard.WizardAbilities.Add( spell );
            CompAbilityUserWizard.wizardAbilitiesByLevel[ spell.abilityLevel-1 ].Add(spell);
        }

        public override void PostInitialize() {
            base.PostInitialize();

            // populate this at start
//            Log.Message("CompAbilityUserWizard.PostInitialize 1");
            if ( CompAbilityUserWizard.WizardAbilities == null ) {
                CompAbilityUserWizard.WizardAbilities = new List<UMAbilityDef>();
                CompAbilityUserWizard.wizardAbilitiesByLevel = new List<UMAbilityDef>[CompAbilityUserWizard.MaxWizardSpellLevel];
                for(int i =0;i<CompAbilityUserWizard.MaxWizardSpellLevel;i++) {
                    CompAbilityUserWizard.wizardAbilitiesByLevel[i] = new List<UMAbilityDef>();
                }
                CompAbilityUserWizard.InitWizardAbility( UnificaMagicaDefOf.UM_WizardBolt );
                CompAbilityUserWizard.InitWizardAbility( UnificaMagicaDefOf.UM_FearPerson );
                CompAbilityUserWizard.InitWizardAbility( UnificaMagicaDefOf.UM_FearBomb );
            }
            this.UpdateAbilities();
        }

        public override bool TryTransformPawn() {
            Pawn p = this.abilityUser;
            //            Log.Message("CompAblityUserWizard.TryTransformPawn "+p.ThingID+" "+(p.Name != null ? p.Name.ToStringFull:" unnamed"));
            bool retval = false;

            if ( p.story != null && p.story.traits != null) {
                if (p.story.traits.HasTrait(UnificaMagicaDefOf.WizardInclined)) {
                    //                    Log.Message("CompAblityUserWizard.TryTransformPawn true");
                    retval = true;
                }
            }

            return retval;
        }

    }

}
