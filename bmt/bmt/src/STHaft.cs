using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace sometools.src
{
    public class STHaft: Item
    {
        public override string GetHeldItemName(ItemStack itemStack)
        {
            string material = itemStack.Item.LastCodePart();
            if(material.Equals("bone") || material.Equals("stick") || material.Equals("wooden") || material.Equals("hewn") || material.Equals("carpented"))
            {
                return Lang.Get("bmt:material-" + material) + " " + Lang.Get("bmt:haft");
            }
            return Lang.Get("game:material-" + material) + " " + Lang.Get("bmt:haft");
        }
    }
}
