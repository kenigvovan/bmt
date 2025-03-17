using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace sometools.src
{
    public class STToolHead: Item
    {
        public override string GetHeldItemName(ItemStack itemStack)
        {
            string material = itemStack.Item.LastCodePart();
            string toolType = itemStack.Item.FirstCodePart();
            if (material.Equals("bone") || material.Equals("stick") || material.Equals("wooden") || material.Equals("hewn") || material.Equals("carpented"))
            {
                return Lang.Get("bmt:material-" + material) + " " + Lang.Get("bmt:" + toolType);
            }
            return Lang.Get("game:material-" + material) + " " + Lang.Get("bmt:" + toolType);
            //return base.GetHeldItemName(itemStack);
        }
    }
}
