using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.MODEL.Items.Weapons.Visitor;
using projektRPG.MODEL.Players;

namespace projektRPG.MODEL.Items.Weapons
{
    public abstract class HeavyWeapon : Weapon
    {
        public override (int Damage, int Defense) Accept(IAttackVisitor visitor, Player p)
        => visitor.Visit(this, p);

        public override int NoiseRange => 7;
    }
}
