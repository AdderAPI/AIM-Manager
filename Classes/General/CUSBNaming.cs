

namespace AIMAPI.Classes.General
{
    public class CUSBNaming
    {
        public string Name
        {
            get
            {                
                return "C-USB LAN";
            }
        }

        public string GetMethod
        {
            get
            {
                return "get_all_c_usb";
            }
        }

        public string DeleteMethod
        {
            get
            {
                return "delete_c_usb";
            }
        }

        public string UpdateMethod
        {
            get
            {
                return "update_c_usb";
            }
        }

        public string ConnectMethod
        {
            get
            {
                return "connect_c_usb";
            }
        }

        public string DisconnectMethod
        {
            get
            {
                return "disconnect_c_usb";
            }
        }

        public string CountNode
        {
            get
            {
                return "count_c_usbs";
            }
        }

        public string ContainerNode
        {
            get
            {
                return "c_usb_lan_extenders";
            }
        }

        public string ItemNode
        {
            get
            {
                return "c_usb";
            }
        }
    }
}
