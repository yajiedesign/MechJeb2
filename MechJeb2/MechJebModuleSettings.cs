using UnityEngine;

namespace MuMech
{
    class MechJebModuleSettings : DisplayModule
    {
        public MechJebModuleSettings(MechJebCore core) : base(core) 
        { 
            showInEditor = true;
            showInFlight = true;
        }

        [Persistent(pass = (int)Pass.Global)]
        public bool useOldSkin = false;

        public override void OnLoad(ConfigNode local, ConfigNode type, ConfigNode global)
        {
            base.OnLoad(local, type, global);

            if (useOldSkin) GuiUtils.LoadSkin(GuiUtils.SkinType.MechJeb1);
        }

        protected override void WindowGUI(int windowID)
        {
            GUILayout.BeginVertical();

            if (GUILayout.Button("\n恢复出厂默认设置\n"))
            {
                KSP.IO.FileInfo.CreateForType<MechJebCore>("mechjeb_settings_global.cfg").Delete();
                KSP.IO.FileInfo.CreateForType<MechJebCore>("mechjeb_settings_type_" + vessel.vesselName + ".cfg").Delete();
                core.ReloadAllComputerModules();
            }

            if (GuiUtils.skin == null || GuiUtils.skin.name != "KSP window 2")
            {
                GUILayout.Label("当前皮肤: MechJeb 2");
                if (GUILayout.Button("使用 MechJeb 1 皮肤"))
                {
                    GuiUtils.LoadSkin(GuiUtils.SkinType.MechJeb1);
                    useOldSkin = true;
                }
            }
            else
            {
                GUILayout.Label("当前皮肤: MechJeb 1");
                if (GUILayout.Button("使用 MechJeb 2 皮肤"))
                {
                    GuiUtils.LoadSkin(GuiUtils.SkinType.Default);
                    useOldSkin = false;
                }
            }

            GUILayout.EndVertical();

            base.WindowGUI(windowID);
        }

        public override string GetName()
        {
            return "设定";
        }

        public override GUILayoutOption[] WindowOptions()
        {
            return new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(100) };
        }
    }
}
