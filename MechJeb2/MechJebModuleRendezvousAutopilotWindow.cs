using UnityEngine;

namespace MuMech
{
    class MechJebModuleRendezvousAutopilotWindow : DisplayModule
    {
        public MechJebModuleRendezvousAutopilotWindow(MechJebCore core) : base(core) { }

        protected override void WindowGUI(int windowID)
        {
            if (!core.target.NormalTargetExists)
            {
                GUILayout.Label("选择汇合目标.");
                base.WindowGUI(windowID);
                return;
            }

            if (core.target.Orbit.referenceBody != orbit.referenceBody)
            {
                GUILayout.Label("汇合目标必须在同一引力圈内.");
                base.WindowGUI(windowID);
                return;
            }

            GUILayout.BeginVertical();

            MechJebModuleRendezvousAutopilot autopilot = core.GetComputerModule<MechJebModuleRendezvousAutopilot>();
            if (autopilot != null)
            {
                GuiUtils.SimpleLabel("汇合目标", core.target.Name);
                
                if (!autopilot.enabled)
                {
                    if (GUILayout.Button("启动自动驾驶")) autopilot.users.Add(this);
                }
                else
                {
                    if (GUILayout.Button("关闭自动驾驶")) autopilot.users.Remove(this);
                }

                GuiUtils.SimpleTextBox("需要的最终距离:", autopilot.desiredDistance, "m");

                if (autopilot.enabled) GUILayout.Label("状态: " + autopilot.status);
            }

            GUILayout.EndVertical();

            base.WindowGUI(windowID);
        }

        public override GUILayoutOption[] WindowOptions()
        {
            return new GUILayoutOption[] { GUILayout.Width(300), GUILayout.Height(50) };
        }

        public override string GetName()
        {
            return "自动汇合";
        }
    }
}
