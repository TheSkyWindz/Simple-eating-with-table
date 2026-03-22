using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using Verse.Noise;
using Verse.Grammar;
using RimWorld;
using RimWorld.Planet;

using System.Reflection;
using HarmonyLib;

namespace BetterEatingWithoutTable
{
    [DefOf]
    public class AteWithTableDefOf
    {
        public static ThoughtDef AteWithTable;
    }

    [StaticConstructorOnStartup]
    public static class Start
    {
        static Start()
        {
            Harmony harmony = new Harmony("theskywinds.bettereatingwithouttable");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
    
    [HarmonyPatch(typeof(Toils_Ingest), nameof(Toils_Ingest.FinalizeIngest))]
    public static class EatWithTable
    {
        [HarmonyPostfix]
        public static void Postfix(Toil __result, Pawn ingester, TargetIndex ingestibleInd)
        {
            Thing thing = ingester.CurJob.GetTarget(ingestibleInd).Thing;
            __result.AddFinishAction(() =>
            {
                if ((ingester.Position + ingester.Rotation.FacingCell).HasEatSurface(ingester.Map) && ingester.GetPosture() == PawnPosture.Standing && thing.def.ingestible.tableDesired)
                    ingester.needs.mood.thoughts.memories.TryGainMemory(AteWithTableDefOf.AteWithTable);
            });
        }
    }
}