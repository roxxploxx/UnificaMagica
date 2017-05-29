using System;
using UnityEngine;
using Verse;

namespace UnificaMagica
{
	[StaticConstructorOnStartup]
	class UnificaMagicaUI {

        public static readonly Texture2D ArrowRight = ContentFinder<Texture2D>.Get("UI/Widgets/ArrowRight", true);
        public static readonly Texture2D ArrowLeft  = ContentFinder<Texture2D>.Get("UI/Widgets/ArrowLeft", true);
    }
}
