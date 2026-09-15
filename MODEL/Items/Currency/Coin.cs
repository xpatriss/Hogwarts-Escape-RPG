using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.Infrastructure.LogStorage;

namespace projektRPG.MODEL.Items.Currency
{
    public class Coin : Currency
    {
        public override string Name => "Galeon";
        public override char Symbol => 'ʛ';
        public override bool PickUp(Players.Player player)
        {
            player.Coins += 1;
            return true;
        }

    }
}
