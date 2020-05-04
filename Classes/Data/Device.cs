using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIMAPI.Classes.General;

namespace AIMAPI.Classes.Data
{
    public class Device : Base, IEquatable<Device>
    {
        public string StId = "";
        public string Location = "";
        public string SerialNumber = "";

        public string Version = "";
        public string Variant = "";
        public string Credentials = "";
        public bool Configured = false;
        public bool Trusted = false;
        public string DateAdded = "";
        public string AIMCertificateUpdatedDateTime = "";

        public DeviceInfo DeviceInfo = null;

        /// <summary>
        /// Get/Set the Device type to be either a RX or TX
        /// </summary>
        public DeviceType Type = DeviceType.RX;

        /// <summary>
        /// Get/Set the device status to Offline, Online, Rebooting, Upgrading, None, RunningBackupFirmware
        /// </summary>
        public DeviceStatus Status = DeviceStatus.Offline;

        // Firmware
        public string FirmwareMain = "";
        public string FirmwareBackup = "";
        public bool ValidFirmware = false;
        public bool ValidBackupFirmware = false;

        // Network
        public string IPAddress1 = "";
        public string IPAddress2 = "";
        public string MacAddress0 = "";
        public string MacAddress1 = "";
        public string MacAddress2 = "";
        public bool Online1 = false;
        public bool Online2 = false;
        public string PreferredIPAddress = "";

        //The following are Transmitter Only
        public int CountTransmitterChannels = 0;
        public int CountTransmitterPresets = 0;

        //The following are Receiver Only
        public bool ConnectionExclusive = false;
        public string ConnectionControl = "";  //1-Video-only, 2-Exclusive mode, 3-Shared mode
        public string ConnectionStartTime = "";
        public string ConnnectionEndTime = "";
        public string Username = "";
        public string UserId = "";
        public string ChannelName = "";
        public int CountReceiverGroups = 0;
        public int CountReceiverPresets = 0;
        public int CountUsers = 0;

        public bool Equals(Device other)
        {
            if (other == null)
                return false;

            return
                Id == other.Id &&
                StId == other.StId &&
                Name == other.Name &&
                Description == other.Description &&
                Location == other.Location &&

                Version == other.Version &&
                Variant == other.Variant &&
                Credentials == other.Credentials &&
                Configured == other.Configured &&
                Trusted == other.Trusted &&
                DateAdded == other.DateAdded &&
                AIMCertificateUpdatedDateTime == other.AIMCertificateUpdatedDateTime &&

                Type == other.Type &&
                Status == other.Status &&

                // Firmware
                FirmwareMain == other.FirmwareMain &&
                FirmwareBackup == other.FirmwareBackup &&
                ValidFirmware == other.ValidFirmware &&
                ValidBackupFirmware == other.ValidBackupFirmware &&

                // Network
                IPAddress1 == other.IPAddress1 &&
                IPAddress2 == other.IPAddress2 &&
                MacAddress0 == other.MacAddress0 &&
                MacAddress1 == other.MacAddress1 &&
                MacAddress2 == other.MacAddress2 &&
                Online1 == other.Online1 &&
                Online2 == other.Online2 &&
                PreferredIPAddress == other.PreferredIPAddress &&

                //The following are Transmitter Only
                CountTransmitterChannels == other.CountTransmitterChannels &&
                CountTransmitterPresets == other.CountTransmitterPresets &&

                //The following are Receiver Only
                ConnectionExclusive == other.ConnectionExclusive &&
                ConnectionControl == other.ConnectionControl && 
                ConnectionStartTime == other.ConnectionStartTime && 
                ConnnectionEndTime == other.ConnnectionEndTime &&
                Username == other.Username &&
                UserId == other.UserId &&
                ChannelName == other.ChannelName &&
                CountReceiverGroups == other.CountReceiverGroups &&
                CountReceiverPresets == other.CountReceiverPresets &&
                CountUsers == other.CountUsers;

        }
    }
}
