using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using AIMAPI.Classes.Data;
using AIMAPI.Classes.General;

namespace AIMAPI.Classes.Parsers
{
    public class DeviceParser : SuccessParser
    {       
        public List<Device> Devices = new List<Device>();

        public DeviceParser(string xml) : base (xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNodeList nodes = doc.GetElementsByTagName("api_response");            

            foreach (XmlNode node in nodes)
            {
                if (_errorcode == string.Empty)
                {
                    XmlNode devices = node.SelectSingleNode("devices");
                    if (devices != null)
                    {
                        foreach (XmlNode devicenode in devices)
                        {
                            Device device = new Device();

                            device.Xml = devicenode.InnerXml;
                            device.APIVersion = _version;

                            device.Id = GetNode(devicenode,"d_id");
                            device.SerialNumber = GetNode(devicenode, "d_serial_number");

                            device.MacAddress0 = GetNode(devicenode, "d_mac_address0");
                            device.MacAddress1 = GetNode(devicenode, "d_mac_address");
                            device.MacAddress2 = GetNode(devicenode, "d_mac_address2");

                            device.Name = GetNode(devicenode, "d_name");
                            device.Online1 = Str2Bool(GetNode(devicenode, "d_online"));
                            device.Online2 = Str2Bool(GetNode(devicenode, "d_online2"));

                            string type = GetNode(devicenode, "d_type");
                            if (type == "rx") device.Type = DeviceType.RX; else device.Type = DeviceType.TX;                            
                            
                            device.Version = GetNode(devicenode, "d_version");
                            device.Variant = GetNode(devicenode, "d_variant");
                            device.Credentials = GetNode(devicenode, "d_credentials");

                            device.IPAddress1 = GetNode(devicenode, "d_ip_address");
                            device.IPAddress2 = GetNode(devicenode, "d_ip_address2");

                            device.Description = GetNode(devicenode, "d_description");
                            device.Location = GetNode(devicenode, "d_location");

                            device.Configured = Str2Bool(GetNode(devicenode, "d_configured"));
                            device.Trusted = Str2Bool(GetNode(devicenode, "d_trusted"));

                            device.ValidFirmware = Str2Bool(GetNode(devicenode, "d_valid_firmware"));
                            device.ValidBackupFirmware = Str2Bool(GetNode(devicenode, "d_valid_backup_firmware"));

                            device.FirmwareMain = GetNode(devicenode, "d_firmware");
                            device.FirmwareBackup = GetNode(devicenode, "d_backup_firmware");

                            device.DateAdded = GetNode(devicenode, "d_date_added");
                            device.AIMCertificateUpdatedDateTime = GetNode(devicenode, "d_aim_certificates_updated");

                            device.Status = GetStatus(GetNode(devicenode, "d_status"));

                            device.PreferredIPAddress = GetNode(devicenode, "preferred_ip");

                            device.StId = GetNode(devicenode, "st_d_id");
                            
                            //// Transmitter Only
                            device.CountTransmitterChannels = Str2Int(GetNode(devicenode, "count_transmitter_channels"));
                            device.CountTransmitterPresets = Str2Int(GetNode(devicenode, "count_transmitter_presets"));

                            //// Receiver Only
                            device.ConnectionExclusive = Str2Bool(GetNode(devicenode, "con_exclusive"));
                            device.ConnectionControl = GetNode(devicenode, "con_control");
                            device.ConnectionStartTime = GetNode(devicenode, "con_start_time");
                            device.ConnnectionEndTime = GetNode(devicenode, "con_end_time");

                            device.Username = GetNode(devicenode, "u_username");
                            device.UserId = GetNode(devicenode, "u_id");

                            device.ChannelName = GetNode(devicenode, "c_name");
                            device.CountReceiverGroups = Str2Int(GetNode(devicenode, "count_receiver_groups"));
                            device.CountReceiverPresets = Str2Int(GetNode(devicenode, "count_receiver_presets"));
                            device.CountUsers = Str2Int(GetNode(devicenode, "count_users"));

                            device.DeviceInfo = new DeviceInfo(device.Type, device.Version, device.Variant);

                            Devices.Add(device);
                        }
                    }
                }
            }
        }

        private DeviceStatus GetStatus(String value)
        {
            // (0 = device offline, 1 = device online, 2 = rebooting, 4 = firmware_upgrading, 6 = running backup firmware)
            DeviceStatus result = DeviceStatus.Offline;
            if (value == "0") result = DeviceStatus.Offline;
            if (value == "1") result = DeviceStatus.Online;
            if (value == "2") result = DeviceStatus.Rebooting;
            if (value == "4") result = DeviceStatus.Upgrading;
            if (value == "6") result = DeviceStatus.RunningBackupFirmware;
            return result;
        }
    }
}
