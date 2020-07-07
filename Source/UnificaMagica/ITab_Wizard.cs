using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace UnificaMagica
{
    public class ITab_Pawn_Wizard : ITab
    {
        private Pawn PawnToShowInfoAbout
        {
            get
            {
                Pawn pawn = null;
                if (base.SelPawn != null)
                {
                    pawn = base.SelPawn;
                }
                else
                {
                    Corpse corpse = base.SelThing as Corpse;
                    if (corpse != null)
                    {
                        pawn = corpse.InnerPawn;
                    }
                }
                if (pawn == null)
                {
                    Log.Error("Character tab found no selected pawn to display.");
                    return null;
                }
                return pawn;
            }
        }

        public override bool IsVisible
        {
            get
            {
                // Log.Message("ITab_isvisible");
                ThingWithComps selected = this.SelThing as ThingWithComps;
                if ( selected != null ) {
                    CompAbilityUserWizard w = selected.GetComp<CompAbilityUserWizard>();
                    if (  w != null && w.IsInitialized ) {
                        // Log.Message("ITab Visible");
                        this.labelKey = w.LabelKey; // defined by the Comp
                        return true;
                    }
                }
                return false;
            }
        }

        public ITab_Pawn_Wizard()
        {
            // Log.Message("ITab_ToggleDef 1");
            this.size = WizardCardUtility.CardSize + new Vector2(17f, 17f) * 2f;
            this.labelKey = "UM_TabToggleDef";
            // Log.Message("ITab_ToggleDef 2");
            /*
            ThingWithComps selected = Find.Selector.SingleSelectedThing as ThingWithComps;
            Log.Message("ITab_ToggleDef 3");
            CompToggleDef td = selected.GetComp<CompToggleDef>();
            Log.Message("ITab_ToggleDef 4");
            if ( td == null ) { Log.Warning("selected thing has no CompToggleDef for ITab_ToggleDef"); }
            Log.Message("ITab_ToggleDef 5");
            this.labelKey = ((CompProperties_ToggleDef)td.props).labelKey;//"UM_TabToggleDef";//.Translate();
            Log.Message("ITab_ToggleDef 6");
            if ( this.labelKey == null ) this.labelKey = "TOGGLEDEF";
            Log.Message("ITab_ToggleDef 7");
            */
        }


        // TODO - place into ITab_Pawn_UM class
        protected override void FillTab() // Rect rect, ThingWithComps selectedThing, ref int curShownLevel)
        {
            Log.Message("ITab FillTab");
            Rect rect = new Rect(17f, 17f, WizardCardUtility.CardSize.x, WizardCardUtility.CardSize.y);
            WizardCardUtility.DrawCard(rect, this.PawnToShowInfoAbout, ref this.curShownLevel);

        }


        protected int curShownLevel = 1;
        /* protected override void FillTab()
        {
            ThingWithComps selected = Find.Selector.SingleSelectedThing as ThingWithComps;
            CompAbilityUserWizard w = selected.GetComp<CompAbilityUserWizard>();
            if ( w == null ) { Log.Warning("selected thing has no CompAbilityUserWizard for ITab_Wizard"); }
            this.labelKey = "UM_Wizard";//((CompProperties_W)td.props).labelKey;//"UM_TabToggleDef";//.Translate();
//            if ( this.labelKey == null ) this.labelKey = "TOGGLEDEF";
            Rect rect = new Rect(17f, 17f, WizardCardUtility.CardSize.x, WizardCardUtility.CardSize.y);

            // GUI.color = Color.black;
            // TODO
            WizardCardUtility.DrawCard(rect, selected, ref curShownLevel);
        }*/
    }
}
