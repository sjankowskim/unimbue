using ThunderRoad;
using HarmonyLib;
using UnityEngine;
using System.Collections;

namespace UnImbue
{
    public class UnImbue : LevelModule
    {
        private static ColliderGroup imbueColliderGroup;

        public override IEnumerator OnLoadCoroutine()
        {
            new Harmony("OnTriggerImbue").PatchAll();
            return base.OnLoadCoroutine();
        }

        [HarmonyPatch(typeof(SpellCaster), "OnTriggerImbue")]
        class TriggerPatch
        {
            public static void Postfix(Collider other, bool enter)
            {
                if (other.GetComponentInParent<ColliderGroup>()?.imbue != null)
                    imbueColliderGroup = enter ? other.GetComponentInParent<ColliderGroup>() : null;
            }
        }

        public override void Update()
        {
            base.Update();
            if (Player.currentCreature != null)
            {
                if (!Player.currentCreature.mana.casterLeft.isFiring && !Player.currentCreature.mana.casterRight.isFiring && imbueColliderGroup != null)
                    imbueColliderGroup = null;

                if (Player.currentCreature.mana.casterLeft.spellInstance is SpellCastCharge leftCharge)
                    CheckCharge(leftCharge, Side.Left);

                if (Player.currentCreature.mana.casterRight.spellInstance is SpellCastCharge rightCharge)
                    CheckCharge(rightCharge, Side.Right);
            }
        }

        private void CheckCharge(SpellCastCharge charge, Side side)
        {
            if (imbueColliderGroup != null
                && ((side == Side.Left) ? Player.currentCreature.mana.casterLeft.isFiring : Player.currentCreature.mana.casterRight.isFiring)
                && PlayerControl.GetHand(side).gripPressed
                && !Player.currentCreature.equipment.GetHeldWeapon(side))
            {
                if (charge != null && charge.imbueRate > 0 && charge.id.Equals(imbueColliderGroup.imbue.spellCastBase?.id))
                {
                    Player.currentCreature.mana.casterLeft.manaWaste = -Player.currentCreature.mana.casterLeft.manaWaste;
                    charge.imbueRate = -charge.imbueRate;
                }
            }
            else if (imbueColliderGroup == null || (charge != null && !PlayerControl.GetHand(side).gripPressed))
            {
                if (charge != null && charge.imbueRate < 0)
                {
                    Player.currentCreature.mana.casterLeft.manaWaste = -Player.currentCreature.mana.casterLeft.manaWaste;
                    charge.imbueRate = -charge.imbueRate;
                }
            }
        }
    }
}
