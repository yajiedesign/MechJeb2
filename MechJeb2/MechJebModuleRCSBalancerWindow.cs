using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MuMech
{
    public class MechJebModuleRCSBalancerWindow : DisplayModule
    {
        public MechJebModuleRCSBalancer balancer;

        public override void OnStart(PartModule.StartState state)
        {
            balancer = core.GetComputerModule<MechJebModuleRCSBalancer>();

            if (balancer.smartTranslation)
            {
                balancer.users.Add(this);
            }

            base.OnStart(state);
        }

        private void SimpleTextInfo(string left, string right)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(left, GUILayout.ExpandWidth(true));
            GUILayout.Label(right, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
        }

        protected override void WindowGUI(int windowID)
        {
            GUILayout.BeginVertical();

            bool wasEnabled = balancer.smartTranslation;

            GUILayout.BeginHorizontal();
            balancer.smartTranslation = GUILayout.Toggle(balancer.smartTranslation, "智能平移", GUILayout.Width(130));
            GUIStyle s = new GUIStyle(GUI.skin.label);
            s.normal.textColor = Color.yellow;
            GUILayout.Label("实验", s);
            GUILayout.EndHorizontal();

            if (wasEnabled != balancer.smartTranslation)
            {
                balancer.ResetThrusterForces();

                if (balancer.smartTranslation)
                {
                    balancer.users.Add(this);
                }
                else
                {
                    balancer.users.Remove(this);
                }
            }

            if (balancer.smartTranslation)
            {
                // Overdrive
                double oldOverdrive = balancer.overdrive;
                double oldOverdriveScale = balancer.overdriveScale;
                double oldFactorTorque = balancer.tuningParamFactorTorque;
                double oldFactorTranslate = balancer.tuningParamFactorTranslate;
                double oldFactorWaste = balancer.tuningParamFactorWaste;

                GuiUtils.SimpleTextBox("过载", balancer.overdrive, "%");

                double sliderVal = GUILayout.HorizontalSlider((float)balancer.overdrive, 0.0F, 1.0F);
                int sliderPrecision = 3;
                if (Math.Round(Math.Abs(sliderVal - oldOverdrive), sliderPrecision) > 0)
                {
                    double rounded = Math.Round(sliderVal, sliderPrecision);
                    balancer.overdrive = new EditableDoubleMult(rounded, 0.01);
                }

                GUILayout.Label("增加功率,但是消耗更多燃料");

                // Advanced options
                balancer.advancedOptions = GUILayout.Toggle(balancer.advancedOptions, "高级选项");
                if (balancer.advancedOptions)
                {
                    // This doesn't work properly, and it might not even be needed.
                    //balancer.smartRotation = GUILayout.Toggle(balancer.smartRotation, "Smart rotation");

                    GuiUtils.SimpleTextBox("过载比例", balancer.overdriveScale);
                    GuiUtils.SimpleTextBox("扭矩系数", balancer.tuningParamFactorTorque);
                    GuiUtils.SimpleTextBox("平移系数", balancer.tuningParamFactorTranslate);
                    GuiUtils.SimpleTextBox("消耗系数", balancer.tuningParamFactorWaste);
                }

                // Apply tuning parameters.
                if (oldOverdrive != balancer.overdrive
                    || oldOverdriveScale != balancer.overdriveScale
                    || oldFactorTorque != balancer.tuningParamFactorTorque
                    || oldFactorTranslate != balancer.tuningParamFactorTranslate
                    || oldFactorWaste != balancer.tuningParamFactorWaste)
                {
                    balancer.UpdateTuningParameters();
                }
            }

            if (balancer.smartTranslation)
            {
                balancer.users.Add(this);
            }
            else
            {
                balancer.users.Remove(this);
            }

            GUILayout.EndVertical();

            base.WindowGUI(windowID);
        }

        public override GUILayoutOption[] WindowOptions()
        {
            return new GUILayoutOption[] { GUILayout.Width(240), GUILayout.Height(30) };
        }

        public override string GetName()
        {
            return "RCS 平衡器";
        }

        public MechJebModuleRCSBalancerWindow(MechJebCore core) : base(core) { }
    }
}
