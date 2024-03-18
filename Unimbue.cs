using ThunderRoad;
using HarmonyLib;
using System;

namespace Unimbue
{
    public class Unimbue : ThunderScript
    {
        public override void ScriptEnable()
        {
            base.ScriptEnable();
            new Harmony("Transfer").PatchAll();
        }

        [HarmonyPatch(typeof(Imbue), "Transfer", new Type[] { typeof(SpellCastCharge), typeof(float) })]
        class TransferPatch
        {
            public static bool Prefix(Imbue __instance, SpellCastCharge spellCastBase, float energyTransfered)
            {
                return !(__instance.spellCastBase == null && spellCastBase.id.Equals("Unimbue"));
            }
        }
    }
}
