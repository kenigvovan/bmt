using bmt.src;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace sometools.src
{
    public class bmt : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            api.RegisterItemClass("CustomItemAxe", typeof(CustomItemAxe));
            api.RegisterItemClass("CustomItemHammer", typeof(CustomItemHammer));
            api.RegisterItemClass("CustomItemHoe", typeof(CustomItemHoe));
            api.RegisterItemClass("CustomItemKnife", typeof(CustomItemKnife));
            api.RegisterItemClass("CustomItemPickaxe", typeof(CustomItemPickaxe));
            api.RegisterItemClass("CustomItemSaw", typeof(CustomItemSaw));
            api.RegisterItemClass("CustomItemProspectingPick", typeof(CustomItemProspectingPick));
            api.RegisterItemClass("CustomItemScythe", typeof(CustomItemScythe));
            api.RegisterItemClass("CustomItemShovel", typeof(CustomItemShovel));
            api.RegisterItemClass("STHaft", typeof(STHaft));
            api.RegisterItemClass("STToolHead", typeof(STToolHead));

        }

        public override void StartServerSide(ICoreServerAPI api)
        {

        }

        public override void StartClientSide(ICoreClientAPI api)
        {

        }
    }
}
