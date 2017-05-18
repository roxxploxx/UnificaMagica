using System;
using System.Collections.Generic;
using Verse;

namespace UnificaMagica
{
    // used to track non-wizard penalties due to using wizard items
    public class CompProperties_WizardTech : CompProperties
    {
        //
        public CompProperties_WizardTech()
        {
            this.compClass = typeof(CompWizardTech);
        }

    }
}
