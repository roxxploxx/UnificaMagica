// using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace UnificaMagica
{
    public class Building_ArcaneAlcove : Building_WorkTable
    {
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {

            foreach (FloatMenuOption c in base.GetFloatMenuOptions(selPawn))
            {
                yield return c;
            }

            if (selPawn.RaceProps.Humanlike && !selPawn.Drafted && base.Faction == Faction.OfPlayer)
            {

                CompAbilityUserWizard compWizard = selPawn.TryGetComp<CompAbilityUserWizard>();
                if ( compWizard == null  ) yield break;

                Action meditate = delegate
                {
                    if (selPawn.CanReserveAndReach(this, PathEndMode.ClosestTouch, Danger.Deadly))
                    {
//                        compForce.canMeditateTicks = Find.TickManager.TicksGame + 6000;
                        Job newJob = new Job(DefDatabase<JobDef>.GetNamed("UM_StudyWizardryJob"), this);
                        selPawn.jobs.TryTakeOrderedJob(newJob);
                        selPawn.mindState.ResetLastDisturbanceTick();
                    }
                };

                if (!selPawn.CanReserve(this))
                {
                    yield return new FloatMenuOption("UM_StudyWizardryJob".Translate() + " (" + "Reserved".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
                    //yield return new FloatMenuOption("PJ_ForceMeditate".Translate() + " (" + "Reserved".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
                }
                else if ( !compWizard.IsInitialized) // || (compForce != null && compForce.ForceUserLevel < 1))
                {
                    yield return new FloatMenuOption("UM_StudyWizardryJob".Translate() + " (" + "UM_StudyWizardryJob_WizardsOnly".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
                }
//                else if (compForce != null && compForce.canMeditateTicks > Find.TickManager.TicksGame)
//                {
//                    yield return new FloatMenuOption("UM_StudyWizardryJob".Translate() + " (" + "PJ_ForceMeditate_NeedRest".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
//                }
                else
                {
                    yield return new FloatMenuOption("UM_StudyWizardryJob".Translate(), meditate, MenuOptionPriority.Default, null, null, 0f, null, null);
                }

                //if (Verse.DebugSettings.godMode)
                //{
                //    yield return new FloatMenuOption("DEBUG" + "PJ_ForceMeditate".Translate(), meditate, MenuOptionPriority.Default, null, null, 0f, null, null);
                //}
            }
        }
    }
}
