using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using AIMAPI.Classes.Data;

namespace AIMAPI.Classes.Parsers
{
    public class PresetParser : PageParser
    {
        public List<Preset> Presets = new List<Preset>();

        public PresetParser(string xml) : base(xml, null)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNodeList nodes = doc.GetElementsByTagName("api_response");

            foreach (XmlNode node in nodes)
            {
                if (_errorcode == string.Empty)
                {
                    XmlNode presets = node.SelectSingleNode("connection_presets");
                    if (presets != null)
                    {
                        foreach (XmlNode presetnode in presets)
                        {
                            Preset preset = new Preset();

                            preset.Xml = presetnode.InnerXml;
                            preset.APIVersion = _version;

                            preset.Id = GetNode(presetnode, "cp_id");
                            preset.Name = GetNode(presetnode, "cp_name");
                            
                            preset.Description = GetNode(presetnode, "cp_description");
                            preset.Pairs = Str2Int(GetNode(presetnode, "cp_pairs"));
                            preset.ProblemPairs = Str2Int(GetNode(presetnode, "problem_cp_pairs"));

                            preset.Active = GetActiveType(GetNode(presetnode, "cp_active"));
                            preset.RXCount = Str2Int(GetNode(presetnode, "connected_rx_count"));

                            preset.ViewButton = GetViewType(GetNode(presetnode, "view_button"));
                            preset.SharedButton = GetViewType(GetNode(presetnode, "shared_button"));
                            preset.ExclusiveButton = GetViewType(GetNode(presetnode, "exclusive_button"));
                            preset.ControlButton = GetViewType(GetNode(presetnode, "control_button"));

                            Presets.Add(preset);
                        }
                    }
                }
            }
        }

        private ViewType GetViewType(String value)
        {
            value = value.ToLower();
            ViewType result = ViewType.Disabled;
            if (value == "disabled") result = ViewType.Disabled;
            if (value == "enabled") result = ViewType.Enabled;
            if (value == "hidden") result = ViewType.Hidden;
            return result;
        }

        private ActiveType GetActiveType(String value)
        {
            // (values ‘full’, ‘partial’, and ‘none’))
            value = value.ToLower();
            ActiveType result = ActiveType.None;
            if (value == "full") result = ActiveType.Full;
            if (value == "partial") result = ActiveType.Partial;
            return result;
        }
    }
}
