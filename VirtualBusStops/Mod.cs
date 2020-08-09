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
                       panel.relativePosition = new Vector2(1315, 0);
                       panel.canFocus = true;
                       panel.size = new Vector2(490, 100);

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
                       seginput.width = 90f;
                       seginput.height = 25f;
                       seginput.padding = new RectOffset(6, 6, 6, 6);
                       seginput.relativePosition = new Vector3(115, 50);

                       segNoneButton = UIUtils.CreateButton(panel);
                       segNoneButton.text = "None";
                       segNoneButton.textScale = 1f;
                       segNoneButton.relativePosition = new Vector2(210, 50);
                       segNoneButton.width = 70;

                       segNoneButton.eventClick += (c, p) =>
                       {
                           ResetFlags();
                       };

                       segLeftButton = UIUtils.CreateButton(panel);
                       segLeftButton.text = "StopLeft";
                       segLeftButton.textScale = 1f;
                       segLeftButton.relativePosition = new Vector2(280, 50);
                       segLeftButton.width = 70;

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
                       segRightButton.relativePosition = new Vector2(355, 50);
                       segRightButton.width = 70;

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
                       segAllButton.relativePosition = new Vector2(425, 50);
                       segAllButton.width = 70;

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