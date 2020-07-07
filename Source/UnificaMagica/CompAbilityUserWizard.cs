using RimWorld;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using Verse;
using AbilityUser;
using UnityEngine;


namespace UnificaMagica
{
    [CompilerGenerated]
    [Serializable]
    [StaticConstructorOnStartup]
    public class CompAbilityUserWizard : AbilityUser.CompAbilityUser
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
            
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (respawningAfterLoad)
            {
                // Log.Message("CompAblityUserWizard PostSpawnSetup " + respawningAfterLoad);
                this.Initialize();
            }
        }

        public int WizardryLevel {
            get {
                return this.AbilityUser.skills.GetSkill(UnificaMagicaDefOf.Wizardry).Level;
            }
        }

        public int NumSelectedInLevel(int lvl) {
            int retval = 0;
            foreach ( PawnAbility pa in this.AbilityData.Powers ) {
                UMAbilityDef wad = pa.Def as UMAbilityDef;
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
            foreach (PawnAbility pa in this.AbilityData.Powers ) { if ( pa.Def == ab ) retval++; }
            return retval;
        }

        // Reduces one power's cooldown by their level. if lvl is not provided, look it up
        public bool StudyForATick(int lvl = -1) {
            bool retval = false;
            Log.Message("styd for a tick 1");
            if (lvl == -1) {
                Log.Message("styd for a tick 1.1");
                lvl = this.AbilityUser.skills.GetSkill(UnificaMagicaDefOf.Wizardry).Level;
                Log.Message("styd for a tick 1.2");
            }
            Log.Message("styd for a tick 2");

            foreach ( AbilityUser.PawnAbility pa in this.AbilityData.Powers ) {
                Log.Message("styd for a tick 2.1 - " + pa);
                if ( pa.CooldownTicksLeft > 0 ) {
                    Log.Message("styd for a tick 2.1.1");
                    pa.CooldownTicksLeft -= lvl;
                    retval = true;
                    break;
                } // only do 1
                Log.Message("styd for a tick 2.2");
            }
            Log.Message("styd for a tick 3");
            return retval;
        }


        // Ordered by level, list of all abilities
        public static List<UMAbilityDef> WizardAbilities= null;   // static for class

        // Ordered by level, list of all abilities, of a certain level
        protected static List<UMAbilityDef>[] wizardAbilitiesByLevel = null; // static for class

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


        // adds spells to wizard at initialization
        protected static void InitWizardAbility(UMAbilityDef spell) {
            CompAbilityUserWizard.WizardAbilities.Add( spell );
            CompAbilityUserWizard.wizardAbilitiesByLevel[ spell.abilityLevel-1 ].Add(spell);
        }

        public bool IsWizard
        {
            get
            {
                bool retval = false;

                if (base.AbilityUser != null && base.AbilityUser.story != null &&
                    base.AbilityUser.story.traits.HasTrait(UnificaMagicaDefOf.WizardInclined) ) {
                    retval = true;
                }
                return retval;
            }
        }
        public override void PostDraw()
        {
            if (this.IsWizard)
            {
                if (base.AbilityUser.IsColonist && this.IsWizard)
                {
                    DrawWizardMark();
                }
            }
        }

        // From TMMagic mostly
        public void DrawWizardMark()
        {
            float num = Mathf.Lerp(1.2f, 1.55f, 1f);
            Vector3 vector = this.AbilityUser.Drawer.DrawPos;
            vector.x = vector.x + .45f;
            vector.z = vector.z + .45f;
            vector.y = Altitudes.AltitudeFor(AltitudeLayer.MoteOverhead);
            float angle = 0f;
            Vector3 s = new Vector3(.28f, 1f, .28f);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, wizardMarkMat, 0);
        }
        public static readonly Material wizardMarkMat = MaterialPool.MatFrom("Other/WizardMark", ShaderDatabase.Transparent, Color.blue);

        public override void PostInitialize() {
            base.PostInitialize();

            // Log.Message("PostInitialize 1");
            if (base.AbilityUser != null)
            {
                // Log.Message("PostInitialize 1.1");
                if (base.AbilityUser.Spawned)
                {
                    // Log.Message("PostInitialize 1.1.1");
                    if (base.AbilityUser.story != null && this.IsWizard )
                    {
                        // Log.Message("Hit resolve");
                        // this.Initialize();
                        this.ResolveTab();
                        this.ResolveWizardAbilities();
                        this.UpdateAbilities();
                    }
                    // Log.Message("PostInitialize 1.1.2");
                }
                // Log.Message("PostInitialize 1.2");
            }
            // Log.Message("PostInitialize 2");
        }

        public void ResolveWizardAbilities() {
            // populate this the first time a wizard needs to be inited
            if ( CompAbilityUserWizard.wizardAbilitiesByLevel == null ) {
                CompAbilityUserWizard.WizardAbilities = new List<UMAbilityDef>();
                CompAbilityUserWizard.wizardAbilitiesByLevel = new List<UMAbilityDef>[CompAbilityUserWizard.MaxWizardSpellLevel];
                for(int i =0;i<CompAbilityUserWizard.MaxWizardSpellLevel;i++) {
                    CompAbilityUserWizard.wizardAbilitiesByLevel[i] = new List<UMAbilityDef>();
                }

                // Level 1
                CompAbilityUserWizard.InitWizardAbility( UnificaMagicaDefOf.UM_WizardBolt );
                CompAbilityUserWizard.InitWizardAbility( UnificaMagicaDefOf.UM_WizardSparks);

                // Level 2
                CompAbilityUserWizard.InitWizardAbility( UnificaMagicaDefOf.UM_WizardBlink);
                CompAbilityUserWizard.InitWizardAbility( UnificaMagicaDefOf.UM_FearPerson );

                // Level 3
                CompAbilityUserWizard.InitWizardAbility( UnificaMagicaDefOf.UM_FearBomb );

                // First pass
                
            }

            this.UpdateAbilities();
        }

        public void ResolveTab()
        {
            // Log.Message("ResolveTab");
            // From TMMagic
            InspectTabBase inspectTabsx = base.AbilityUser.GetInspectTabs().FirstOrDefault((InspectTabBase x) => x.labelKey == "UM_TabToggleDef");
            IEnumerable<InspectTabBase> inspectTabs = base.AbilityUser.GetInspectTabs();
            bool flag = inspectTabs != null && inspectTabs.Count<InspectTabBase>() > 0;
            if (flag)
            {
                if (inspectTabsx == null)
                {
                    try
                    {
                        base.AbilityUser.def.inspectorTabsResolved.Add(InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Wizard)));
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Concat(new object[]
                        {
                            "Could not instantiate inspector tab of type ",
                            typeof(ITab_Pawn_Wizard),
                            ": ",
                            ex
                        }));
                    }
                }
            }
        }

        public override bool TryTransformPawn() {
            Pawn p = this.AbilityUser;
            // Log.Message("CompAblityUserWizard.TryTransformPawn "+p.ThingID+" "+(p.Name != null ? p.Name.ToStringFull:" unnamed"));
            bool retval = false;

            if ( p.story != null && p.story.traits != null) {
                if (p.story.traits.HasTrait(UnificaMagicaDefOf.WizardInclined)) {
                    Log.Message("CompAblityUserWizard.TryTransformPawn true");
                    retval = true;
                }
            }

            //Log.Warning("CompAbilityUserWizard.TryTransformPawn is true alwasy for debugging");
            //retval = true;

            return retval;
        }

    }

}
