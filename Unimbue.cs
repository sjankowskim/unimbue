using ThunderRoad;

namespace Unimbue
{
    public class UnimbueSpell : SpellCastCharge
    {
    }

    public class Unimbue : ThunderScript
    {
        public override void ScriptUpdate()
        {
            base.ScriptUpdate();

            foreach (Item item in Item.all)
            {
                foreach (Imbue imbue in item.imbues)
                {
                    if (imbue.spellCastBase != null && imbue.spellCastBase.id.Equals("Unimbue"))
                    {
                        imbue.energy = 0;
                    }
                }
            }
        }
    }
}
