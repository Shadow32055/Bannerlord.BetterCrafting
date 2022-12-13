using BetterCrafting.Utils;
using BetterCrafting.ViewModels;
using HarmonyLib;
using SandBox.GauntletUI;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ScreenSystem;

namespace BetterCrafting
{
    public class SubModule : MBSubModuleBase
    {

        private static GauntletLayer? layer = null;
        public static ScreenBase? sbase;
        public static CraftingMultiplyerViewModel datasource = new CraftingMultiplyerViewModel();

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            Harmony h = new Harmony("Bannerlord.Shadow.BetterCrafting");

            h.PatchAll();
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();

        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            string modName = base.GetType().Assembly.GetName().Name;

            Helper.SetModName(modName);
        }

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);

            sbase = ScreenManager.TopScreen;

            if (sbase != null)
            {

                if (sbase is GauntletCraftingScreen)
                {

                    if (layer == null)
                    {
                        layer = new GauntletLayer(100, "GauntletLayer", true);
                        layer.LoadMovie("CraftingHUD", datasource);
                        sbase.AddLayer(layer);
                        ScreenManager.TrySetFocus(layer);
                    }

                    if (Input.IsKeyDown(InputKey.LeftControl) && Input.IsKeyDown(InputKey.LeftShift))
                    {
                        datasource.SetMultiplier(100);
                    }
                    else if (Input.IsKeyDown(InputKey.LeftShift))
                    {
                        datasource.SetMultiplier(10);
                    }
                    else if (Input.IsKeyDown(InputKey.LeftControl))
                    {
                        datasource.SetMultiplier(5);
                    }
                    else
                    {
                        datasource.SetMultiplier(1);
                    }

                }
                else if (layer != null)
                {
                    //Helper.DisplayFriendlyMsg("Removing layer");
                    sbase.RemoveLayer(layer);
                    layer = null;
                }
            }
        }
    }
}