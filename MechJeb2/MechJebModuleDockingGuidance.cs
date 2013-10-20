using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MuMech
{
    public class MechJebModuleDockingGuidance : DisplayModule
    {
        public MechJebModuleDockingGuidance(MechJebCore core) : base(core) { }

        MechJebModuleDockingAutopilot autopilot;

        public override void OnStart(PartModule.StartState state)
        {
            autopilot = core.GetComputerModule<MechJebModuleDockingAutopilot>();
        }

        protected override void WindowGUI(int windowID)
        {
            if (!core.target.NormalTargetExists)
            {
                GUILayout.Label("选择一个对接的目标");
                base.WindowGUI(windowID);
                return;
            }

            GUILayout.BeginVertical();

            // GetReferenceTransformPart is null after undocking ...
            if (vessel.GetReferenceTransformPart() == null || !vessel.GetReferenceTransformPart().Modules.Contains("ModuleDockingNode"))
            {
                GUIStyle s = new GUIStyle(GUI.skin.label);
                s.normal.textColor = Color.yellow;
                GUILayout.Label("警告：您需要从一个对接端口控制船只。右键点击一个对接端口，并选择 \"Control from here\"", s);
            }

            if (!(core.target.Target is ModuleDockingNode))
            {
                GUIStyle s = new GUIStyle(GUI.skin.label);
                s.normal.textColor = Color.yellow;
                GUILayout.Label("警告：目标是不是一个对接端口。右键单击目标的对接端口和选择 \"Set as target\"", s);
            }

            bool onAxisNodeExists = false;
            foreach (ModuleDockingNode node in vessel.GetModules<ModuleDockingNode>())
            {
                if (Vector3d.Angle(node.GetTransform().forward, vessel.ReferenceTransform.up) < 2)
                {
                    onAxisNodeExists = true;
                    break;
                }
            }

            if (!onAxisNodeExists)
            {
                GUIStyle s = new GUIStyle(GUI.skin.label);
                s.normal.textColor = Color.yellow;
                GUILayout.Label("警告：此船是不是从对接节点控制。这个船只上右键单击所需的对接节点并选择 \"Control from here.\"", s);
            }

            bool active = GUILayout.Toggle(autopilot.enabled, "启动自动驾驶仪");
            GuiUtils.SimpleTextBox("速度限制", autopilot.speedLimit, "m/s");
			
            if (autopilot.speedLimit < 0)
                autopilot.speedLimit = 0;


            GUILayout.BeginHorizontal();
            autopilot.forceRol = GUILayout.Toggle(autopilot.forceRol, "力矩 :", GUILayout.ExpandWidth(false));

            autopilot.rol.text = GUILayout.TextField(autopilot.rol.text, GUILayout.Width(30));
            GUILayout.Label("°", GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();

            if (autopilot.enabled != active)
            {
                if (active)
                {
                    autopilot.users.Add(this);
                }
                else
                {
                    autopilot.users.Remove(this);
                }
            }

            if (autopilot.enabled)
            {
                GUILayout.Label("状态: " + autopilot.status);
                Vector3d error = core.rcs.targetVelocity - vesselState.velocityVesselOrbit;
                double error_x = Vector3d.Dot(error, vessel.GetTransform().right);
                double error_y = Vector3d.Dot(error, vessel.GetTransform().forward);
                double error_z = Vector3d.Dot(error, vessel.GetTransform().up);
                GUILayout.Label("错误 X: " + error_x.ToString("F2") + " m/s  [L/J]");
                GUILayout.Label("错误 Y: " + error_y.ToString("F2") + " m/s  [I/K]");
                GUILayout.Label("错误 Z: " + error_z.ToString("F2") + " m/s  [H/N]");
            }

            GUILayout.EndVertical();

            base.WindowGUI(windowID);
        }

        public override GUILayoutOption[] WindowOptions()
        {
            return new GUILayoutOption[] { GUILayout.Width(300), GUILayout.Height(50) };
        }

        public override void OnModuleDisabled()
        {
            if (autopilot != null) autopilot.users.Remove(this);
        }

        public override string GetName()
        {
            return "自动对接";
        }
    }
}
