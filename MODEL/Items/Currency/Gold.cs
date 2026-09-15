using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.Infrastructure.LogStorage;

namespace projektRPG.MODEL.Items.Currency
{
    public class Gold : Currency
    {
        public override string Name => "Gold";
        public override char Symbol => '⬥';

        public override bool PickUp(Players.Player player)
        {
            player.Gold += 1;
            return true;
        }
    }
}
