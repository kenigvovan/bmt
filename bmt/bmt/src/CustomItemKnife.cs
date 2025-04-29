using Newtonsoft.Json.Linq;
using sometools.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace bmt.src
{
    public class CustomItemKnife: ItemKnife, IContainedMeshSource
    {
        public string Construction
        {
            get
            {
                return this.Variant["construction"];
            }
        }
        private Dictionary<int, MultiTextureMeshRef> meshrefs
        {
            get
            {
                return ObjectCacheUtil.GetOrCreate<Dictionary<int, MultiTextureMeshRef>>(this.api, "bmtknivesmeshrefs", () => new Dictionary<int, MultiTextureMeshRef>());
            }
        }
        private ICoreClientAPI capi;
        private Dictionary<string, float> durabilitybyhandlers = new Dictionary<string, float>();
        private Dictionary<string, float> miningspeedbyhandlers = new Dictionary<string, float>();
        private Dictionary<string, float> durabilitybybindings = new Dictionary<string, float>();
        private Dictionary<string, float> miningspeedbybindings = new Dictionary<string, float>();
        private Dictionary<string, float> bonusdurability = new Dictionary<string, float>();
        private Dictionary<string, float> bonusspped = new Dictionary<string, float>();
        public override void OnCreatedByCrafting(ItemSlot[] allInputslots, ItemSlot outputSlot, GridRecipe byRecipe)
        {
            base.OnCreatedByCrafting(allInputslots, outputSlot, byRecipe);
            string material = "copper";
            string handle = "stick";
            string worktype = "smelted";
            string stri = "none";
            string bonus = "none";
            foreach (var it in allInputslots)
            {
                if (it.Empty)
                {
                    continue;
                }
                if (it.Itemstack.Item != null && it.Itemstack.Item.GetType() == typeof(STToolHead))
                {
                    material = it.Itemstack.Item.LastCodePart();
                    worktype = it.Itemstack.Item.LastCodePart(1);
                }
                else if (it.Itemstack.Item != null && it.Itemstack.Item.GetType() == typeof(STHaft))
                {
                    handle = it.Itemstack.Item.LastCodePart();
                }
                else if (it.Itemstack.Block != null && it.Itemstack.Block.Code.Path.StartsWith("wildvine-"))
                {
                    stri = "vine";
                }
                else if (it.Itemstack.Item != null && it.Itemstack.Item.Code.Path.StartsWith("flaxtwine-"))
                {
                    stri = "twine";
                }
                else if (it.Itemstack.Item != null && it.Itemstack.Item.Code.Path.Equals("leather-normal-plain"))
                {
                    stri = "leather";
                }
                else if (it.Itemstack.Item != null && it.Itemstack.Item.Code.Path.Equals("leather-sturdy-plain"))
                {
                    stri = "rleather";
                }
                else if (it.Itemstack.Item != null && it.Itemstack.Item.Code.Path.StartsWith("metalnailsandstrips-"))
                {
                    stri = "metal";
                }
                else if (it.Itemstack.Item != null && it.Itemstack.Item.Code.Path.Equals("rope"))
                {
                    stri = "metal";
                }
            }

            JsonItemStack jsonItemStack = new JsonItemStack();
            jsonItemStack.Code = new AssetLocation("bmt:knife-generic-" + material);
            jsonItemStack.Type = EnumItemClass.Item;
            jsonItemStack.Attributes = new JsonObject(JToken.Parse(string.Format(
                "{{ material: \"{0}\", handle: \"{1}\", worktype: \"{2}\", stri: \"{3}\", bonus: \"{4}\" }}"
                , material, handle, worktype, stri, bonus)));
            var c = jsonItemStack.Resolve(this.api.World, "bmt knife", true);
            outputSlot.Itemstack = jsonItemStack.ResolvedItemstack.Clone();
            outputSlot.MarkDirty();
        }
        public override float GetMiningSpeed(IItemStack itemstack, BlockSelection blockSel, Block block, IPlayer forPlayer)
        {
            string bonus = itemstack.Attributes.GetString("bonus", "none");
            string handle = itemstack.Attributes.GetString("handle", "stick");
            string stri = itemstack.Attributes.GetString("string", "none");
            float rateSpeed = 1;
            rateSpeed *= miningspeedbyhandlers.Get(handle, 1);
            rateSpeed *= miningspeedbybindings.Get(stri, 1);
            rateSpeed *= bonusspped.Get(bonus, 1);
            return base.GetMiningSpeed(itemstack, blockSel, block, forPlayer) * rateSpeed;
        }
        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
            string material = inSlot.Itemstack.Item.LastCodePart();
            string worktype = inSlot.Itemstack.Attributes.GetString("worktype", "smelted");
            string bonus = inSlot.Itemstack.Attributes.GetString("bonus", "none");
            string handle = inSlot.Itemstack.Attributes.GetString("handle", "stick");
            string stri = inSlot.Itemstack.Attributes.GetString("stri", "none");

            if (!bonus.Equals("none") && !stri.Equals("none"))
            {
                dsc.Append(Lang.Get("A {0} {1} Knife with a {2} handle and {3} bindings", Lang.Get("bmt:bonus-" + bonus),
                                                                                            Lang.Get("game:material-" + material),
                                                                                            Lang.Get("bmt:handle-" + handle),
                                                                                            Lang.Get("bmt:binding-" + stri)));
                return;
            }
            else if (bonus.Equals("none") && !stri.Equals("none"))
            {
                dsc.Append(Lang.Get("A {0} Knife with a {1} handle and {2} bindings", Lang.Get("game:material-" + material),
                                                                                            Lang.Get("bmt:handle-" + handle),
                                                                                            Lang.Get("bmt:binding-" + stri)));
                return;
            }
            else if (!bonus.Equals("none") && stri.Equals("none"))
            {
                dsc.Append(Lang.Get("A {0} {1} Knife with a {2} handle", Lang.Get("bmt:bonus-" + bonus),
                                                                                            Lang.Get("game:material-" + material),
                                                                                            Lang.Get("bmt:handle-" + handle)));
                return;
            }
            else if (bonus.Equals("none") && stri.Equals("none"))
            {
                dsc.Append(Lang.Get("A {0} Knife with a {1} handle", Lang.Get("game:material-" + material),
                                                                                            Lang.Get("bmt:handle-" + handle)));
                return;
            }
        }
        private JsonItemStack genJstack(string json)
        {
            JsonItemStack jsonItemStack = new JsonItemStack();
            jsonItemStack.Code = this.Code;
            jsonItemStack.Type = EnumItemClass.Item;
            jsonItemStack.Attributes = new JsonObject(JToken.Parse(json));
            jsonItemStack.Resolve(this.api.World, "bmt knife", true);
            return jsonItemStack;
        }
        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
            this.durabilitybyhandlers = this.Attributes["durabilitybyhandlers"].AsObject<Dictionary<string, float>>(null);
            this.miningspeedbyhandlers = this.Attributes["miningspeedbyhandlers"].AsObject<Dictionary<string, float>>(null);
            this.durabilitybybindings = this.Attributes["durabilitybybindings"].AsObject<Dictionary<string, float>>(null);
            this.miningspeedbybindings = this.Attributes["miningspeedbybindings"].AsObject<Dictionary<string, float>>(null);
            this.bonusdurability = this.Attributes["durabilitybyhandlers"].AsObject<Dictionary<string, float>>(null);
            this.bonusspped = this.Attributes["durabilitybyhandlers"].AsObject<Dictionary<string, float>>(null);
            AddAllTypesToCreativeInventory();
        }
        public void AddAllTypesToCreativeInventory()
        {
            /*if (this.Construction == "crude" || this.Construction == "blackguard")
            {
                return;
            }*/
            List<JsonItemStack> stacks = new List<JsonItemStack>();
            //this.Construction
            string construction = this.Construction;
            Dictionary<string, string[]> vg = this.Attributes["variantGroups"].AsObject<Dictionary<string, string[]>>(null);
            foreach (string worktype in vg["worktype"])
            {
                stacks.Add(this.genJstack(string.Format("{{material: \"{0}\", worktype: \"{1}\", bonus: \"{2}\", handle: \"{3}\", stri: \"{4}\"  }}", construction, worktype, vg["bonus"][0],
                    vg["handle"][0], vg["string"][0])));
                /*foreach (string bonus in vg["bonus"])
                {
                    foreach (string handle in vg["handle"])
                    {
                        foreach (string stri in vg["string"])
                        {
                            stacks.Add(this.genJstack(string.Format("{{material: \"{0}\", worktype: \"{1}\", bonus: \"{2}\", handle: \"{3}\", stri: \"{4}\"  }}", construction, worktype, bonus, handle, stri)));
                        }
                    }
                }*/
            }

            this.CreativeInventoryStacks = new CreativeTabAndStackList[]
            {
                new CreativeTabAndStackList
                {
                    Stacks = stacks.ToArray(),
                    Tabs = new string[]
                    {
                        "general",
                        "items"
                    }
                }
            };
        }
        public override void OnBeforeRender(ICoreClientAPI capi, ItemStack itemstack, EnumItemRenderTarget target, ref ItemRenderInfo renderinfo)
        {
            int meshrefid = itemstack.TempAttributes.GetInt("meshRefId", 0);
            if (meshrefid == 0 || !this.meshrefs.TryGetValue(meshrefid, out renderinfo.ModelRef))
            {
                int id = this.meshrefs.Count + 1;
                MultiTextureMeshRef modelref = capi.Render.UploadMultiTextureMesh(this.GenMesh(itemstack, capi.ItemTextureAtlas, null));
                renderinfo.ModelRef = (this.meshrefs[id] = modelref);
                itemstack.TempAttributes.SetInt("meshRefId", id);
            }
            base.OnBeforeRender(capi, itemstack, target, ref renderinfo);
        }
        public MeshData GenMesh(ItemStack itemstack, ITextureAtlasAPI targetAtlas, BlockPos atBlockPos)
        {
            ContainedTextureSource cnts = new ContainedTextureSource(this.api as ICoreClientAPI, targetAtlas, new Dictionary<string, AssetLocation>(), string.Format("For render in bmt:knife {0}", this.Code));
            cnts.Textures.Clear();


            string material = itemstack.Attributes.GetString("material", null);
            if (material == null)
            {
                material = itemstack.Item.LastCodePart();
            }
            string worktype = itemstack.Attributes.GetString("worktype", "smelted");
            string bonus = itemstack.Attributes.GetString("bonus", "none");
            string handle = itemstack.Attributes.GetString("handle", "stick");
            string stri = itemstack.Attributes.GetString("stri", "none");

            string shapeName;
            if (worktype.Equals("hammered") && bonus.Equals("reinforced"))
            {
                shapeName = "rtools/knife";
            }
            else if (worktype.Equals("smelted"))
            {
                shapeName = "tools/knife-copper";
            }
            else
            {
                shapeName = "tools/knife-iron";
            }

            Shape shapeKnife = (this.api as ICoreClientAPI).Assets.TryGet("bmt:shapes/item/" + shapeName + ".json").ToObject<Shape>();
            cnts.Textures["string"] = new AssetLocation("bmt:binding/" + stri + ".png");
            cnts.Textures["material"] = new AssetLocation("game:block/metal/ingot/" + material + ".png");
            cnts.Textures["handle"] = new AssetLocation("bmt:handle/" + handle + ".png");
            //"charcoal": "game:block/coal/charcoal"
            cnts.Textures["charcoal"] = new AssetLocation("game:block/coal/charcoal.png");
            /*foreach (KeyValuePair<string, AssetLocation> ctex in this.capi.TesselatorManager.GetCachedShape(this.Shape.Base).Textures)
            {
                cnts.Textures[ctex.Key] = ctex.Value;
            }*/
            //void TesselateShape(string typeForLogging, Shape shapeBase, out MeshData modeldata, ITexPositionSource texSource, Vec3f meshRotationDeg = null, int generalGlowLevel = 0, byte climateColorMapId = 0, byte seasonColorMapId = 0, int? quantityElements = null, 
            MeshData meshBlade;
            (api as ICoreClientAPI).Tesselator.TesselateShape("item shape", shapeKnife, out meshBlade, cnts, null, 0, 0, 0, null, null);
            return meshBlade;
        }
        public override int GetMaxDurability(ItemStack itemstack)
        {
            string bonus = itemstack.Attributes.GetString("bonus", "none");
            string handle = itemstack.Attributes.GetString("handle", "stick");
            string stri = itemstack.Attributes.GetString("stri", "none");
            float rateDurability = 1;
            rateDurability *= durabilitybyhandlers.Get(handle, 1);
            rateDurability *= durabilitybybindings.Get(stri, 1);
            rateDurability *= bonusdurability.Get(bonus, 1);
            return (int)(base.GetMaxDurability(itemstack) * rateDurability);
        }
        public string GetMeshCacheKey(ItemStack itemstack)
        {
            string wood = itemstack.Attributes.GetString("wood", null);
            string metal = itemstack.Attributes.GetString("metal", null);
            string color = itemstack.Attributes.GetString("color", null);
            string deco = itemstack.Attributes.GetString("deco", null);
            return string.Concat(new string[]
            {
                this.Code.ToShortString(),
                "-",
                wood,
                "-",
                metal,
                "-",
                color,
                "-",
                deco
            });
        }
    }
}
