//using System.Core;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;

namespace UnificaMagica
{
    public class JobDriver_StudyWizardry : JobDriver
    {
        private Rot4 faceDir;

        // This class generates a list of Toils (a new one each time as per the 'yield' command)
        // that are managed by the JobDriver in the Pawn_JobTracker.
        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            Log.Message("MakeNewToils");

            // yield return Toils_Reserve.Reserve(TargetIndex.A, 1);
//            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            yield return new Toil
            {
                

                initAction = delegate
                {
                    Log.Message("MakeNewToils Toil - initAction");
                    this.faceDir = ((!this.HaveCurToil) ? Rot4.Random : this.pawn.CurJob.def.faceDir);

                },
                tickAction = delegate
                {
                    Log.Message("MakeNewToils Toil - tickAction");
                    this.pawn.rotationTracker.FaceCell(this.pawn.Position + this.faceDir.FacingCell);
                    Log.Message("MakeNewToils Toil - tickAction 1");
                    this.pawn.rotationTracker.FaceCell(this.faceDir.FacingCell);
                    // this.pawn.Drawer.rotator.FaceCell(this.pawn.Position + this.faceDir.FacingCell);
                    Log.Message("MakeNewToils Toil - tickAction 1a");
                    this.pawn.GainComfortFromCellIfPossible();
                    Log.Message("MakeNewToils Toil - tickAction 2");
                    if (this.pawn.TryGetComp<CompAbilityUserWizard>() != null)
                    {
                        Log.Message("MakeNewToils Toil - tickAction 2.1");
                        CompAbilityUserWizard wizardComp = this.pawn.GetComp<CompAbilityUserWizard>();
                        //int lvl = this.pawn.skills.GetSkill(UnificaMagicaDefOf.Wizardry).Level;
                        Log.Message("MakeNewToils Toil - tickAction 2.2");
                        Log.Message("found Powers " +wizardComp);
                        Log.Message("lvl " + wizardComp.WizardryLevel);
                        Log.Message("MakeNewToils Toil - tickAction 2.2a");
                        Log.Message("wizardComp is " + wizardComp.ToString());
                        Log.Message("MakeNewToils Toil - tickAction 2.2b");
                        try
                        {
                            wizardComp.StudyForATick();
                        }
                        catch (System.Exception ex)
                        {
                            Log.Message("Caught exception:");
                            Log.Message(ex.ToString());
                        }
                        // Log.Message("found Powers " + wizardComp.Powers.Count);
                        bool flag = wizardComp.StudyForATick(); // true if finds one to work on
                        Log.Message("MakeNewToils Toil - tickAction 2.3");
                        this.pawn.skills.Learn(SkillDefOf.Intellectual, SkillTuning.XpPerTickDefault/2.0f, false);
                        Log.Message("MakeNewToils Toil - tickAction 2.4");
                        this.pawn.skills.Learn(SkillDefOf.Crafting, SkillTuning.XpPerTickDefault / 2.0f, false);
                        Log.Message("MakeNewToils Toil - tickAction 2.5");
                        if ( !flag ) { Log.Message("MakeNewToils Toil - tickAction 2.5.1"); this.EndJobWith(JobCondition.Succeeded); }
                        Log.Message("MakeNewToils Toil - tickAction 2.6");
                    }
                    Log.Message("MakeNewToils Toil - tickAction 3");
                },
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = 1800
            };
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Rot4>(ref this.faceDir, "faceDir", default(Rot4), false);
        }

        public override bool TryMakePreToilReservations(bool _errorOnFailed)
        {
            // Log.Message("TryMakePreToilReservations: "+ _errorOnFailed+ this.job.targetA);
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, _errorOnFailed);
        }
    }
}
