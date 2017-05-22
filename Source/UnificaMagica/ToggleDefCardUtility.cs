using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace UnificaMagica
{
    public class ToggleDefCardUtility
    {
        // RimWorld.CharacterCardUtility
        public static Vector2 CardSize = new Vector2(395f, 536f);

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

        // RimWorld.CharacterCardUtility
        public static void DrawCard(Rect rect, ThingWithComps selectedThing )
        {
            GUI.BeginGroup(rect);

            CompToggleDef compToggleDef = selectedThing.GetComp<CompToggleDef>();

            if (compToggleDef != null)
            {
                float ts = Text.CalcSize("Placeholder").x;
                float y = rect.y;
                Rect rect2 = new Rect(((rect.width / 2) - ts) + SpacingOffset, y, rect.width, HeaderSize);
                y += (float) rect2.height;
                Text.Font = GameFont.Medium;
                Widgets.Label(rect2, "Placeholder".CapitalizeFirst());
                Text.Font = GameFont.Small;
                Widgets.ListSeparator(ref y, rect2.width,"Wearable Locations");

//                List<BodyPartRecord> bodyPartRecord = BodyDefOf.Human.AllParts;


                // add all the buttons for the toggle defs
                foreach (ThingDef td in compToggleDef.toggleDefs)  {
                    Rect rect3 = new Rect(0f,y, rect.width, 20f);
                    bool isactive = false;
                    if ( selectedThing.def == td ) isactive = true;
                    if ( Widgets.RadioButtonLabeled(rect3, td.LabelCap, isactive) ) {
                        //Log.Message(".. change location to "+td.LabelCap);

                        // CHange def then give it a new id. Hopefully nothing index on the id
                        Map map = selectedThing.Map;
                        IntVec3 loc = selectedThing.Position;
                        selectedThing.DeSpawn();
                        selectedThing.def = td;
                        selectedThing.thingIDNumber = -1;
                        ThingIDMaker.GiveIDTo(selectedThing); // necessary
                        GenSpawn.Spawn(selectedThing,loc,map);
                        //Log.Message("id is now "+selectedThing.thingIDNumber+" so hopefully not -1");
                        //works but:
                        //JobDriver threw exception in initAction. Pawn=Jon, Job=Wear A=Thing_Apparel_RingOfWarmth_TOGGLEDEF_L23077, Exception: System.Collections.Generic.KeyNotFoundException: The given key was not present in the dictionary.
                        //so need to change key

                        break;
                    }
                    y+= 25f;
                }
            }

            GUI.EndGroup();
        }

    }
}
