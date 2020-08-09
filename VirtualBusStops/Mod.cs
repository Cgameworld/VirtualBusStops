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
        private UIButton segLeftButton;
        private UIButton segRightButton;
        private UIButton segAllButton;

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
                       panel.relativePosition = new Vector2(1275, 0);
                       panel.canFocus = true;
                       panel.size = new Vector2(340, 140);

                       m_title = panel.AddUIComponent<UITitleBar>();
                       m_title.title = "Virtual Bus Stops";
                       m_title.closeButton.isVisible = false;

                       UILabel seglabel = panel.AddUIComponent<UILabel>();
                       seglabel.text = "Segment ID:";
                       seglabel.autoSize = false;
                       seglabel.width = 105f;
                       seglabel.height = 20f;
                       seglabel.relativePosition = new Vector2(10, 55);

                       seginput = UIUtils.CreateTextField(panel);
                       seginput.text = "";
                       seginput.width = 100f;
                       seginput.height = 25f;
                       seginput.padding = new RectOffset(6, 6, 6, 6);
                       seginput.relativePosition = new Vector3(120, 50);

                       segNoneButton = UIUtils.CreateButton(panel);
                       segNoneButton.text = "None";
                       segNoneButton.textScale = 1f;
                       segNoneButton.relativePosition = new Vector2(240, 50);
                       segNoneButton.width = 90;

                       segNoneButton.eventClick += (c, p) =>
                       {
                           ResetFlags();
                       };

                       segLeftButton = UIUtils.CreateButton(panel);
                       segLeftButton.text = "StopLeft";
                       segLeftButton.textScale = 1f;
                       segLeftButton.relativePosition = new Vector2(10, 90);
                       segLeftButton.width = 100;

                       segLeftButton.eventClick += (c, p) =>
                       {
                           ResetFlags();
                           var segmentid = ushort.Parse(seginput.text);
                           var flags = NetManager.instance.m_segments.m_buffer[segmentid].m_flags;
                           NetManager.instance.m_segments.m_buffer[segmentid].m_flags |= NetSegment.Flags.StopLeft;
                           NetManager.instance.UpdateSegmentRenderer(segmentid, true);
                       };

                       segRightButton = UIUtils.CreateButton(panel);
                       segRightButton.text = "StopRight";
                       segRightButton.textScale = 1f;
                       segRightButton.relativePosition = new Vector2(120, 90);
                       segRightButton.width = 105;

                       segRightButton.eventClick += (c, p) =>
                       {
                           ResetFlags();
                           var segmentid = ushort.Parse(seginput.text);
                           var flags = NetManager.instance.m_segments.m_buffer[segmentid].m_flags;
                           NetManager.instance.m_segments.m_buffer[segmentid].m_flags |= NetSegment.Flags.StopRight;
                           NetManager.instance.UpdateSegmentRenderer(segmentid, true);
                       };

                       segAllButton = UIUtils.CreateButton(panel);
                       segAllButton.text = "StopBoth";
                       segAllButton.textScale = 1f;
                       segAllButton.relativePosition = new Vector2(225, 90);
                       segAllButton.width = 110;

                       segAllButton.eventClick += (c, p) =>
                       {
                           ResetFlags();
                           var segmentid = ushort.Parse(seginput.text);
                           var flags = NetManager.instance.m_segments.m_buffer[segmentid].m_flags;
                           NetManager.instance.m_segments.m_buffer[segmentid].m_flags |= NetSegment.Flags.StopBoth;
                           NetManager.instance.UpdateSegmentRenderer(segmentid, true);
                       };

                   }
               };

            }


        }

        private void ResetFlags()
        {
            Debug.Log("id" + ushort.Parse(seginput.text));
            var flags = NetManager.instance.m_segments.m_buffer[int.Parse(seginput.text)].m_flags;
            flags.ClearFlags(NetSegment.Flags.StopAll);
            flags.ClearFlags(NetSegment.Flags.StopLeft);
            flags.ClearFlags(NetSegment.Flags.StopRight);
            NetManager.instance.UpdateSegmentFlags(ushort.Parse(seginput.text));
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