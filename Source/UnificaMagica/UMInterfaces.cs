
using System;
//using System.Text;
using System.Collections.Generic;
//using System.Diagnostics;
using Verse;
using RimWorld;

namespace UnificaMagica
{

    // ExtendedThings are extended with Extended Components (ExtComp and ExtInstComp)
    public interface IExtendedThing {
        List<ExtInstComp> ExtComps {
            get;
            set;
        }
        ExtThingDef def {
            get;
            set;
        }

        // initialize ExtComps when creating and loading
        void PostMake();
        void ExposeData();
    }

    // <summary>When facilities change, this gets notified.</summary>
    public interface IFacilityChangeNotifier {
        void Notify_NewLink(UMCompFacility _f);
        void Notify_LinkRemoved(UMCompFacility _f);
    }
}
