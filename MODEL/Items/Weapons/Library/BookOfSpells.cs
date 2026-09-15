using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.MODEL.Items.Weapons;

namespace projektRPG.MODEL.Items.Weapons.Library
{
    public class BookOfSpells : MagicWeapon
    {
        public override string Name => "Book of Spells";
        public override char Symbol => 's';

        public override int RequiredHands => 2;
        public override int Damage => 5;
    }
}
