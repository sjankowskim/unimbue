using ThunderRoad;
using HarmonyLib;
using UnityEngine;
using System.Collections;

namespace UnImbue
{
    public class UnImbue : LevelModule
    {
        private static Item targetWep;
        private SpellCastCharge leftCharge, rightCharge;

        public override IEnumerator OnLoadCoroutine()
        {
            Debug.Log("(Unimbue) Loaded successfully!");
            new Harmony("OnTriggerImbue").PatchAll();
            return base.OnLoadCoroutine();
        }

        [HarmonyPatch(typeof(SpellCaster), "OnTriggerImbue")]
        class TriggerPatch
        {
            public static void Postfix(Collider other, bool enter)
            {
                if (other.gameObject.GetComponentInParent<Item>() != null && other.GetComponentInParent<ColliderGroup>().imbue != null)
                {
                    if (enter)
                        targetWep = other.gameObject.GetComponentInParent<Item>();
                    else
                        targetWep = null;
                }
            }
        }

        public override void Update()
        {
            base.Update();
            if (Player.currentCreature != null)
            {
                if (!Player.currentCreature.mana.casterLeft.isFiring && !Player.currentCreature.mana.casterRight.isFiring && targetWep != null)
                    targetWep = null;

                // if imbuing && grip pressed w/ no weapon
                if (targetWep != null &&
                    ((Player.currentCreature.mana.casterLeft.isFiring && PlayerControl.GetHand(Side.Left).gripPressed && !Player.currentCreature.equipment.GetHeldWeapon(Side.Left)) ||
                    (Player.currentCreature.mana.casterRight.isFiring && PlayerControl.GetHand(Side.Right).gripPressed && !Player.currentCreature.equipment.GetHeldWeapon(Side.Right))))
                {
                    leftCharge = (SpellCastCharge)Player.currentCreature.mana.casterLeft.spellInstance;
                    rightCharge = (SpellCastCharge)Player.currentCreature.mana.casterRight.spellInstance;

                    if (leftCharge != null && !leftCharge.id.Equals("Telekenesis") && leftCharge.imbueRate > 0 && WeaponContainsID(leftCharge.id))
                    {
                        Player.currentCreature.mana.casterLeft.manaWaste = -Player.currentCreature.mana.casterLeft.manaWaste;
                        leftCharge.imbueRate = -leftCharge.imbueRate;
                    }
                    if (rightCharge != null && !rightCharge.id.Equals("Telekenesis") && rightCharge.imbueRate > 0 && WeaponContainsID(rightCharge.id))
                    {
                        Player.currentCreature.mana.casterRight.manaWaste = -Player.currentCreature.mana.casterRight.manaWaste;
                        rightCharge.imbueRate = -rightCharge.imbueRate;
                    }
                }
                // if !imbuing || imbuing && !gripPressed
                else if (targetWep == null || (leftCharge != null && !PlayerControl.GetHand(Side.Left).gripPressed) || (rightCharge != null && !PlayerControl.GetHand(Side.Right).gripPressed))
                {
                    if (leftCharge != null && leftCharge.imbueRate < 0)
                    {
                        Player.currentCreature.mana.casterLeft.manaWaste = -Player.currentCreature.mana.casterLeft.manaWaste;
                        leftCharge.imbueRate = -leftCharge.imbueRate;
                    }
                    if (rightCharge != null && rightCharge.imbueRate < 0)
                    {
                        Player.currentCreature.mana.casterRight.manaWaste = -Player.currentCreature.mana.casterRight.manaWaste;
                        rightCharge.imbueRate = -rightCharge.imbueRate;
                    }
                }
            }
        }
        private bool WeaponContainsID(string id)
        {
            if (targetWep != null)
                foreach (Imbue imbue in targetWep.imbues)
                    if (imbue.spellCastBase != null && imbue.spellCastBase.id.Equals(id))
                        return true;
            return false;
        }
    }
}
