using ColossalFramework;
using ColossalFramework.UI;
using Harmony;
using ICities;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace VirtualBusStops
{
    public class ModInfo : IUserMod
    {
        public string Name => "Virtual Bus Stops";

        public string Description => "Test Bus Stops in Road Editor";
    }
    public class ModLoading : LoadingExtensionBase
    {
        private UITextField seginput;
        private UITitleBar m_title;
        private UIButton segNoneButton;
        private UIButton segOneButton;

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            //only start loading in asset editor
            if (mode == LoadMode.LoadAsset || mode == LoadMode.NewAsset)
            {
                GameObject.FindObjectOfType<ToolController>().eventEditPrefabChanged += (info) =>
                {
                    if (info.GetType().ToString() == "NetInfo")
                    {
                        var ifPanel = UIView.Find("VirtualBusStopsMain");
                        if (ifPanel != null)
                        {
                            UIPanel.DestroyImmediate(ifPanel);
                        }

                        UIView view = UIView.GetAView();               
                        UIPanel panel = view.AddUIComponent(typeof(UIPanel)) as UIPanel;
                        panel.name = "VirtualBusStopsMain";
                        panel.atlas = UIUtils.GetAtlas("Ingame");
                        panel.backgroundSprite = "MenuPanel2";
                        panel.relativePosition = new Vector2(1345, 0);
                        panel.canFocus = true;
                        panel.size = new Vector2(320, 100);

                        m_title = panel.AddUIComponent<UITitleBar>();
                        m_title.title = "Virtual Bus Stops";
                        m_title.closeButton.isVisible = false;

                        UILabel seglabel = panel.AddUIComponent<UILabel>();
                        seglabel.text = "Segment ID:";
                        seglabel.autoSize = false;
                        seglabel.width = 105f;
                        seglabel.height = 20f;
                        seglabel.relativePosition = new Vector2(5, 55);

                        seginput = UIUtils.CreateTextField(panel);
                        seginput.text = "";
                        seginput.width = 55f;
                        seginput.height = 25f;
                        seginput.padding = new RectOffset(6, 6, 6, 6);
                        seginput.relativePosition = new Vector3(115, 50);

                        segNoneButton = UIUtils.CreateButton(panel);
                        segNoneButton.text = "None";
                        segNoneButton.textScale = 1f;
                        segNoneButton.relativePosition = new Vector2(170, 50);
                        segNoneButton.width = 70;

                        segOneButton = UIUtils.CreateButton(panel);
                        segOneButton.text = "1Stop";
                        segOneButton.textScale = 1f;
                        segOneButton.relativePosition = new Vector2(240, 50);
                        segOneButton.width = 70;

                    }
                };

            }


        }

    }

    public static class ExtraUtils
    {
        //future refactor here with other methods?
        public static void ShowErrorWindow(string header, string message)
        {
            ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
            panel.SetMessage(header, message, false);
            panel.GetComponentInChildren<UISprite>().spriteName = "IconError";
        }

    }
}