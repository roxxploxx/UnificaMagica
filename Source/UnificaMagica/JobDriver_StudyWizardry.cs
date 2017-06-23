using System;
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
            yield return Toils_Reserve.Reserve(TargetIndex.A, 1);
//            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            yield return new Toil
            {
                initAction = delegate
                {
                    this.faceDir = ((!this.CurJob.def.faceDir.IsValid) ? Rot4.Random : this.CurJob.def.faceDir);

                },
                tickAction = delegate
                {
                    this.pawn.Drawer.rotator.FaceCell(this.pawn.Position + this.faceDir.FacingCell);
                    this.pawn.GainComfortFromCellIfPossible();
                    if (this.pawn.TryGetComp<CompAbilityUserWizard>() != null)
                    {
                        CompAbilityUserWizard wizardComp = this.pawn.GetComp<CompAbilityUserWizard>();
                        //int lvl = this.pawn.skills.GetSkill(UnificaMagicaDefOf.Wizardry).Level;

//                        Log.Message("found Powers " +wizardComp.Powers.Count);
                        bool flag = wizardComp.StudyForATick(); // true if finds one to work on

                        this.pawn.skills.Learn(UnificaMagicaDefOf.Wizardry, LearnRates.XpPerTickDefault , false);
                        if ( !flag ) { this.EndJobWith(JobCondition.Succeeded); }
                    }
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
    }
}
