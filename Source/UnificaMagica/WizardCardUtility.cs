// using Harmony;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//using UnityEngine.Input;
using Verse;
using Verse.Steam;
using RimWorld.IO;


namespace UnificaMagica
{
    public class WizardCardUtility
    {
        // RimWorld.CharacterCardUtility
        public static Vector2 CardSize = new Vector2(395f, 336f);

        public static float ButtonSize = 40f;

        public static float ForceButtonSize = 46f;

        public static float ForceButtonPointSize = 24f;

        public static float HeaderSize = 32f;

        public static float TextSize = 22f;

        public static float Padding = 3f;

        public static float SpacingOffset = 15f;

        public static float SectionOffset = 8f;

        public static float ColumnSize = 245f;

        public static float SkillsColumnHeight = 113f;

        public static float SkillsColumnDivider = 114f;

        public static float SkillsTextWidth = 138f;

        public static float SkillsBoxSize = 18f;

        public static float PowersColumnHeight = 195f;

        public static float PowersColumnWidth = 123f;

        public static bool isfirst = true;

        //        public static List<AbilityDef> lvl1Wizard = null;
        //        public static List<AbilityDef> lvl2Wizard = null;
        //        public static List<AbilityDef> lvl3Wizard = null;

        // OK, think I got this compiling, now run it

        public static void DrawCard(Rect _rect, Pawn _pawn, ref int curShownLevel) {
            Rect rect = new Rect(17f, 17f, WizardCardUtility.CardSize.x, WizardCardUtility.CardSize.y);
            GUI.BeginGroup(rect);

            // TODO: because PawnToShowInfoAbout is private, need to make a ITab_Pawn_UM and then this class inherits from nothing
            CompAbilityUserWizard compWizard = _pawn.GetComp<CompAbilityUserWizard>();

            if ( compWizard != null)
            {
                float ts = Text.CalcSize("Wizard SpellBook").x;
                float y = rect.y;
                Rect rect2 = new Rect(((rect.width / 2) - ts) + SpacingOffset, y, rect.width, HeaderSize);
                y += (float) rect2.height;
                Text.Font = GameFont.Medium;
                Widgets.Label(rect2, "Wizard SpellBook".CapitalizeFirst());
                Text.Font = GameFont.Small;
                Widgets.ListSeparator(ref y, rect2.width,"Select your spells to learn (right click unlearns).");

                // change spell book level
                if ( curShownLevel > 1 )
                {
                    Rect rectarrowL = new Rect(SpacingOffset, y, 25f,25f );
                    if ( Widgets.ButtonImage(rectarrowL,UnificaMagicaUI.ArrowLeft,Color.white) ) {
                        curShownLevel--;
                    }
                }
                Rect rectperlevel = new Rect(25f+SpacingOffset,y,rect.width - 25f - 25f,25f );
                Widgets.Label(rectperlevel,"Level "+curShownLevel+" Spells ("+compWizard.NumSelectedInLevel(curShownLevel)+" of "+compWizard.NumAvailableAtLevel(curShownLevel)+" selected)");
                if ( curShownLevel < CompAbilityUserWizard.MaxWizardSpellLevel ) {
                    Rect rectarrowR = new Rect(rect.width - SpacingOffset - 20f, y, 25f,25f );
                    if ( Widgets.ButtonImage(rectarrowR,UnificaMagicaUI.ArrowRight,Color.white) ) {
                        curShownLevel++;
                    }
                }
                y+= 25f;

                // add all the buttons
                //foreach (ThingDef td in compToggleDef.toggleDefs)
                foreach ( UMAbilityDef ability in CompAbilityUserWizard.GetWizardAbilitiesOfLevel(curShownLevel) ) {
                    Rect rect3 = new Rect(0f,y, rect.width - 25f, 20f);
                    Rect rect3a= new Rect(rect.width-25f,y,25f,20f);
//                    bool isactive = false;
                    // GetNumOfPower(as)
                    if ( Widgets.ButtonText( rect3, ability.LabelCap ) ) {
//                        Log.Message(".. selected ability. right mouse? "+Input.GetMouseButton(1)+ " " +Input.GetMouseButtonDown(1) + " " +Input.GetMouseButtonUp(1));
                        if ( Input.GetMouseButtonUp(1) )  {
                            compWizard.RemovePawnAbility(ability);
                        } else {
                            if ( compWizard.NumSelectedInLevel(curShownLevel) < compWizard.NumAvailableAtLevel(curShownLevel) ){
                                compWizard.AddPawnAbility(ability,false);
                            }
                        }
                        break;
                    }
                    Widgets.Label(rect3a,""+compWizard.GetNumOfAbility(ability));

                    y+= 25f;
                }
            }

            GUI.EndGroup();
            
        }

    }
}
