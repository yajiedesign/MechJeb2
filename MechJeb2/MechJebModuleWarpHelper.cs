using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MuMech
{
    public class MechJebModuleWarpHelper : DisplayModule
    {
        public enum WarpTarget { Periapsis, Apoapsis, Node, SoI }
        static string[] warpTargetStrings = new string[] { "近拱点", "远拱点", "变轨节点", "母星变更" };
        static readonly int numWarpTargets = Enum.GetNames(typeof(WarpTarget)).Length;
        [Persistent(pass = (int)Pass.Global)]
        public WarpTarget warpTarget = WarpTarget.Periapsis;

        [Persistent(pass = (int)Pass.Global)]
        public EditableTime leadTime = 0;

        public bool warping = false;

        protected override void WindowGUI(int windowID)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("加速到: ", GUILayout.ExpandWidth(false));
            warpTarget = (WarpTarget)GuiUtils.ArrowSelector((int)warpTarget, numWarpTargets, warpTargetStrings[(int)warpTarget]);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GuiUtils.SimpleTextBox("前置时间: ", leadTime, "");

            if (warping)
            {
                if (GUILayout.Button("取消"))
                {
                    warping = false;
                    core.warp.MinimumWarp(true);
                }
            }
            else
            {
                if (GUILayout.Button("加速")) warping = true;
            }

            GUILayout.EndHorizontal();

            if (warping) GUILayout.Label("加速到" + (leadTime > 0 ? GuiUtils.TimeToDHMS(leadTime) + " 前 " : "") + warpTargetStrings[(int)warpTarget] + ".");

            GUILayout.EndVertical();

            base.WindowGUI(windowID);
        }

        public override void OnFixedUpdate()
        {
            if (!warping) return;

            double targetUT = vesselState.time;

            switch (warpTarget)
            {
                case WarpTarget.Periapsis:
                    targetUT = orbit.NextPeriapsisTime(vesselState.time);
                    break;

                case WarpTarget.Apoapsis:
                    if (orbit.eccentricity < 1) targetUT = orbit.NextApoapsisTime(vesselState.time);
                    break;

                case WarpTarget.SoI:
                    if (orbit.patchEndTransition != Orbit.PatchTransitionType.FINAL) targetUT = orbit.EndUT;
                    break;

                case WarpTarget.Node:
                    if (vessel.patchedConicSolver.maneuverNodes.Any()) targetUT = vessel.patchedConicSolver.maneuverNodes[0].UT;
                    break;
            }

            targetUT -= leadTime;

            if (targetUT < vesselState.time + 1)
            {
                core.warp.MinimumWarp(true);
                warping = false;
            }
            else
            {
                core.warp.WarpToUT(targetUT);
            }
        }

        public override UnityEngine.GUILayoutOption[] WindowOptions()
        {
            return new GUILayoutOption[] { GUILayout.Width(240), GUILayout.Height(50) };
        }

        public override string GetName()
        {
            return "时间加速";
        }

        public MechJebModuleWarpHelper(MechJebCore core) : base(core) { }
    }
}
