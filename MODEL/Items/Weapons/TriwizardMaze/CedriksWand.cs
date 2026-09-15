using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.MODEL.Items.Weapons;

namespace projektRPG.MODEL.Items.Weapons.TriwizardMaze
{
    public class CedriksWand : MagicWeapon
    {
        public override string Name => "Cedrik's Wand";
        public override char Symbol => '*';

        public override int RequiredHands => 1;
        public override int Damage => 3;

    }
}
