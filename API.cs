using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using AIMAPI.General;
using AIMAPI.Classes.Data;
using AIMAPI.Classes.Parsers;
using System.Timers;
using AIMAPI.Classes.General;


namespace AIMAPI
{
    public class API
    {
        #region Events
        public event EventHandler OnConnecting;
        public event EventHandler OnConnected;
        public event EventHandler OnDisconnected;
        public event EventHandler OnError;
        public event EventHandler OnComplete;
        public event EventHandler OnTokenChanged;

        public event EventHandler OnReceiverAdded;
        public event EventHandler OnReceiverRemoved;
        public event EventHandler OnReceiverUpdated;
        public event EventHandler OnTransmitterAdded;
        public event EventHandler OnTransmitterRemoved;
        public event EventHandler OnTransmitterUpdated;
        public event EventHandler OnChannelAdded;
        public event EventHandler OnChannelRemoved;
        public event EventHandler OnChannelUpdated;
        public event EventHandler OnPresetAdded;
        public event EventHandler OnPresetRemoved;
        public event EventHandler OnPresetUpdated;
        public event EventHandler OnCUSBAdded;
        public event EventHandler OnCUSBRemoved;
        public event EventHandler OnCUSBUpdated;

        public event NotifyCollectionChangedEventHandler OnUpdate;
        #endregion

        private SynchronizationContext _context = null;
        private Connect _connect = new Connect();
        private System.Timers.Timer _timergetupdate = new System.Timers.Timer(10000);
        private Thread _thread;
        private int _pagesize = 100;
        private bool _autoupdate = false;
        private bool _isconnected = false;
        private Fetch _fetch = Fetch.All;
        private CUSBNaming _cusbNaming = new CUSBNaming();

        public ObservableCollection<Device> Receivers = new ObservableCollection<Device>();
        public ObservableCollection<Device> Transmitters = new ObservableCollection<Device>();
        public ObservableCollection<Channel> Channels = new ObservableCollection<Channel>();
        public ObservableCollection<Preset> Presets = new ObservableCollection<Preset>();
        public ObservableCollection<CUSB> CUSB = new ObservableCollection<CUSB>();
        public bool Debug = false;

        /// <summary>
        /// IP Address for the AIM Manager
        /// </summary>
        public string IPAddress
        {
            set { _connect.IP = value; }
            get { return _connect.IP; }
        }

        /// <summary>
        /// HTTP Port number,  default: 80
        /// </summary>
        public int HTTPPort
        {
            set { _connect.HTTPPort = value; }
            get { return _connect.HTTPPort; }
        }

        /// <summary>
        /// UseSSL
        /// </summary>
        public bool UseSSL
        {
            set { _connect.UseSSL = value; }
            get { return _connect.UseSSL; }
        }

        /// <summary>
        /// Username for the AIM Manager
        /// </summary>
        public string Username
        {
            set { _connect.Username = value; }
            get { return _connect.Username; }
        }

        /// <summary>
        /// Password for the AIM Manager
        /// </summary>
        public string Password
        {
            set { _connect.Password = value; }
            get { return _connect.Password; }
        }

        /// <summary>
        /// Get/Set Login Token
        /// </summary>
        public string Token
        {
            set { _connect.Token = value; }
            get { return _connect.Token; }
        }


        /// <summary>
        /// Specifies the results per page.  Minimum = 1 and Maximum = 1000.
        /// </summary>
        public int Pagesize
        {
            set
            {
                _pagesize = value;

                if (_pagesize < 1) _pagesize = 1;
                if (_pagesize > 1000) _pagesize = 1000;
            }
            get { return _pagesize; }
        }

        /// <summary>
        /// The version of the AIM API
        /// </summary>
        public string APIVersion
        {
            get { return _connect.Version; }
        }


        /// <summary>
        /// Enable Auto Update
        /// </summary>
        public bool AutoUpdateEnable
        {
            get { return _autoupdate; }
            set { _autoupdate = value; }
        }

        /// <summary>
        /// Set the Automatic update interval
        /// </summary>
        public double AutoUpdateInterval
        {
            get { return _timergetupdate.Interval; }
            set { _timergetupdate.Interval = value; }
        }

        /// <summary>
        /// Is the Connection Established
        /// </summary>
        public bool IsConnected
        {
            get { return _isconnected; }
        }

        /// <summary>
        /// Connection Retries
        /// </summary>
        public int Retries
        {
            get { return _connect.Retries; }
            set { _connect.Retries = value; }
        }

        /// <summary>
        /// Return Error Message
        /// </summary>
        public ErrorMessage Error
        {
            get
            {
                ErrorMessage msg = new ErrorMessage();
                msg.IsError = _connect.IsError;
                msg.Code = _connect.ErrorCode;
                msg.Message = _connect.ErrorMessage;
                return msg;
            }
        }

        /// <summary>
        /// Allows you to choose what data is retreived from the Manager.  From example, Fetch.All is everything, whilst Fetch.Receiver | Fetch.Transmitters just retrieves the Receivers and Transmitters.
        /// </summary>
        public Fetch Fetch
        {
            set { _fetch = value; }
            get { return _fetch; }
        }

        public API()
        {
            _context = SynchronizationContext.Current; 

            _connect.OnConnecting += new System.EventHandler(ConnectingEvent);
            _connect.OnConnected += new System.EventHandler(ConnectedEvent);
            _connect.OnError += new System.EventHandler(ErrorEvent);
            _connect.OnError += new System.EventHandler(DisconnectedEvent);
            _connect.OnTokenChanged += new System.EventHandler(TokenChangedEvent);

            Receivers.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedEvent);
            Transmitters.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedEvent);
            Presets.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedEvent);
            Channels.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedEvent);
            CUSB.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedEvent);

            _timergetupdate.Elapsed += OnTimerGetUpdate;
        }


        /// <summary>
        /// Get a the Devices, Transmitters, Channels and Presets from the Server
        /// </summary>
        public void Get()
        {
            // Reset Connection State
            _connect.IsError = false;
            _connect.IsDisconnected = false;

            if (_fetch == Fetch.All)
            {
                if (!_connect.Abort) GetReceivers();
                if (!_connect.Abort) GetTransmitters();
                if (!_connect.Abort) GetChannels();
                if (!_connect.Abort) GetPresets();
                if (!_connect.Abort && Useful.Str2Int(APIVersion) >= 6) GetCUSBLans();
                if (!_connect.Abort) CompleteEvent(this, new EventArgs());
            }
            else
            {
                if (!_connect.Abort && _fetch.HasFlag(Fetch.Receivers)) GetReceivers();
                if (!_connect.Abort && _fetch.HasFlag(Fetch.Transmitters)) GetTransmitters();
                if (!_connect.Abort && _fetch.HasFlag(Fetch.Channels)) GetChannels();
                if (!_connect.Abort && _fetch.HasFlag(Fetch.Presets)) GetPresets();
                if (!_connect.Abort && _fetch.HasFlag(Fetch.CUSBLan) && Useful.Str2Int(APIVersion) >= 6) GetCUSBLans();
                if (!_connect.Abort) CompleteEvent(this, new EventArgs());
            }
            

            if (_autoupdate && !_timergetupdate.Enabled)
            {
                _timergetupdate.Enabled = true;
                _isconnected = true;
            }
        }

        /// <summary>
        /// Get a the Devices, Channels and Presets from the Server on a seperate Thread.
        /// </summary>
        public void GetAsync()
        {
            if (_context == null) throw new ArgumentNullException("Context is null");
            GetAsync(_context, _autoupdate);
        }

        /// <summary>
        /// Get a the Devices, Channels and Presets from the Server on a seperate Thread
        /// </summary>
        /// <param name="autoupdate">Enable/Disable AutoUpdate</param>
        public void GetAsync(bool autoupdate)
        {
            if (_context == null) throw new ArgumentNullException("Context is null");
            GetAsync(_context, autoupdate);
        }

        /// <summary>
        /// Get a the Devices, Channels and Presets from the Server on a seperate Thread
        /// </summary>
        /// <param name="context">The thread that is calling the API.  For example: System.Threading.SynchronizationContext.Current</param>
        public void GetAsync(SynchronizationContext context)
        {
            GetAsync(context, _autoupdate);
        }

        /// <summary>
        /// Get a the Devices, Transmitters, Channels and Presets from the Server on a seperate Thread
        /// </summary>
        /// <param name="context">The thread that is calling the API.  For example: System.Threading.SynchronizationContext.Current</param>
        /// <param name="autoupdate">Automatically update the Devices, Transmitters, Channels and Presets lists.  This uses a timer.</param>
        public void GetAsync(SynchronizationContext context, bool autoupdate)
        {
            DebugLog("GetAync");

            _context = context;
            _autoupdate = autoupdate;

            if (_thread == null)
            {
                DebugLog("GetAsync - Start Thread");
                _thread = new Thread(this.Get);
                _thread.IsBackground = true;
                _thread.Name = "GetInfo: " + DateTime.Now.ToLongTimeString();
                _thread.Start();
            }
            else
            {
                DebugLog("GetAsync - Thread already running");
            }

        }

        /// <summary>
        /// Disconnect by Stopping Automatic Update
        /// </summary>
        public void Disconnect()
        {
            DebugLog("Disconnect");

            if (_thread != null)
            {
                _autoupdate = false;
                _timergetupdate.Enabled = false;
                _thread = null;
                _isconnected = false;
            }
            _connect.Disconnect();
            DisconnectedEvent(this, new EventArgs());
        }


        /// <summary>
        /// Clears the data in the Receivers, Transmittersm, Channels, Presets and CUSB collections.
        /// </summary>
        public void Clear()
        {
            lock (this)
            {
                Receivers.Clear();
                Transmitters.Clear();
                Channels.Clear();
                Presets.Clear();
                CUSB.Clear();
            }
        }

        /// <summary>
        /// Login to the API
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Success</returns>
        public bool Login(string username, string password)
        {
            return _connect.Login(username, password);
        }


        /// <summary>
        /// Login to the API
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Success</returns>
        public bool Logout()
        {
            string apiversion = "3";
            string request = "&method=logout";

            _connect.Get(request, apiversion);

            if (_connect.Response != string.Empty)
            {
                SuccessParser parser = new SuccessParser(_connect.Response);
                return parser.Success;
            }

            return false;
        }


        /// <summary>
        /// Connect to Channel
        /// </summary>
        /// <param name="recieverid">Receiver Id</param>
        /// <param name="channelid">Channel id</param>
        /// <param name="connectionmode">Connection Mode:  View Only, Shared, Exclusive & Private</param>
        /// <returns></returns>
        public bool ConnectChannel(string receiverid, string channelid, ConnectionMode connectionmode)
        {
            string mode = string.Empty;
            string apiversion = "";

            if (Useful.Str2Int(APIVersion) < 5)
            {
                apiversion = "3";
                switch (connectionmode)
                {
                    case ConnectionMode.ViewOnly:
                        mode += "&view_only=1";
                        break;
                    case ConnectionMode.Private:
                        mode += "&exclusive=1";
                        break;
                }
            }
            else
            {
                apiversion = "5";
                mode = "&mode=";
                switch (connectionmode)
                {
                    case ConnectionMode.ViewOnly:
                        mode += "v";
                        break;
                    case ConnectionMode.Shared:
                        mode += "s";
                        break;
                    case ConnectionMode.Exclusive:
                        mode += "e";
                        break;
                    case ConnectionMode.Private:
                        mode += "p";
                        break;
                }

                if (connectionmode == ConnectionMode.Default) mode = "";
            }

            string request = "&method=connect_channel&c_id=" + channelid + "&rx_id=" + receiverid + mode;

            _connect.Get(request, apiversion);

            if (_connect.Response != string.Empty)
            {
                SuccessParser parser = new SuccessParser(_connect.Response);
                return parser.Success;
            }

            return false;
        }


        /// <summary>
        /// Disconnect the Channel from the Receiver
        /// </summary>
        /// <param name="receiverid">Receivers Id</param>
        /// <returns>Success</returns>
        public bool DisconnectChannel(string receiverid)
        {
            return DisconnectChannel(receiverid, false);
        }


        /// <summary>
        /// Disconnect the Channel from the Receiver
        /// </summary>
        /// <param name="ReceiverId">Receivers Id</param>
        /// <param name="force">Force the disconnection.  Only Admin can do this.</param>
        /// <returns>Success</returns>
        public bool DisconnectChannel(string ReceiverId, bool force)
        {
            string request = "&method=disconnect_channel";
            if (ReceiverId != "")
                request += "&rx_id=" + ReceiverId;

            if (force) request += "&force=1";

            _connect.Get(request, "3");
            if (_connect.Response != string.Empty && !_connect.IsError)
            {
                SuccessParser parser = new SuccessParser(_connect.Response);
                return parser.Success;
            }

            return false;
        }


        /// <summary>
        /// Disconnect All Channels
        /// </summary>
        /// <returns>Success</returns>
        public bool DisconnectChannelAll()
        {
            return DisconnectChannel("", false);
        }


        /// <summary>
        /// Disconnect All Channels
        /// </summary>
        /// <param name="force">Set "true" to force a disconnection</param>
        /// <returns></returns>
        public bool DisconnectChannelAll(bool force)
        {
            return DisconnectChannel("", force);
        }


        /// <summary>
        /// Connect to a Preset
        /// </summary>
        /// <param name="presetid">Preset Id</param>
        /// <param name="connectionmode">Connection Mode:  View Only, Shared, Exclusive & Private</param>
        /// <returns></returns>
        public bool ConnectPreset(string presetid, ConnectionMode connectionmode)
        {
            return ConnectPreset(presetid, connectionmode, false);
        }

        /// <summary>
        /// Connect to a Preset
        /// </summary>
        /// <param name="presetid">Preset Id</param>
        /// <param name="connectionmode">Connection Mode:  View Only, Shared, Exclusive & Private</param>
        /// <param name="force">Ignore errors with some of the preset’s pairs or not.  Only an Admin can do this.</param>
        /// <returns>Success</returns>
        public bool ConnectPreset(string presetid, ConnectionMode connectionmode, bool force)
        {
            string mode = string.Empty;
            string apiversion = "";

            if (Useful.Str2Int(APIVersion) < 5)
            {
                apiversion = "3";
                switch (connectionmode)
                {
                    case ConnectionMode.ViewOnly:
                        mode += "&view_only=1";
                        break;
                    case ConnectionMode.Private:
                        mode += "&exclusive=1";
                        break;
                }
            }
            else
            {
                apiversion = "5";
                mode += "&mode=";
                switch (connectionmode)
                {
                    case ConnectionMode.ViewOnly:
                        mode += "v";
                        break;
                    case ConnectionMode.Shared:
                        mode += "s";
                        break;
                    case ConnectionMode.Exclusive:
                        mode += "e";
                        break;
                    case ConnectionMode.Private:
                        mode += "p";
                        break;
                }

                if (connectionmode == ConnectionMode.Default) mode = "";
            }

            string request = "&method=connect_preset&id=" + presetid;

            if (force)
                request += "&force=1";

            _connect.Get(request, apiversion);
            if (_connect.Response != string.Empty && !_connect.IsError)
            {
                SuccessParser parser = new SuccessParser(_connect.Response);
                return parser.Success;
            }

            return false;
        }


        /// <summary>
        /// Disconnect a Preset
        /// </summary>
        /// <param name="presetid">Preset Id</param>
        /// <returns>Success</returns>
        public bool DisconnectPreset(string presetid)
        {
            return DisconnectPreset(presetid, false);
        }

        /// <summary>
        /// Disconnect a Preset
        /// </summary>
        /// <param name="presetid">Preset Id</param>
        /// <param name="force">Ignore errors with some of the preset’s pairs or not.  Only an Admin can do this.</param>
        /// <returns>Success</returns>
        public bool DisconnectPreset(string presetid, bool force)
        {
            string request = "&method=disconnect_channel&id=" + presetid;

            if (force) request += "&force=1";

            _connect.Get(request, "3");
            if (_connect.Response != string.Empty && !_connect.IsError)
            {
                SuccessParser parser = new SuccessParser(_connect.Response);
                return parser.Success;
            }

            return false;
        }

        /// <summary>
        /// Create a Preset
        /// </summary>
        /// <param name="name">Preset Name</param>
        /// <param name="description">Preset Description</param>
        /// <param name="presetpairs">List of Receiver and Channel Pairs</param>
        /// <param name="connectionmode">Bitwise: Video Only, Shared, Exclusive or Private (Optional, default is global setting)</param>
        /// <returns>Success</returns>
        public bool CreatePreset(string name, string description, List<PresetPair> presetpairs, ConnectionMode connectionmode)
        {
            name = URLSafe(name);
            description = URLSafe(description);

            string mode = string.Empty;
            string apiversion = "";

            if (Useful.Str2Int(APIVersion) < 5)
            {
                apiversion = "3";
                if (connectionmode != ConnectionMode.Default)
                {
                    if (connectionmode.HasFlag(ConnectionMode.ViewOnly)) mode += "v";
                    if (connectionmode.HasFlag(ConnectionMode.Shared)) mode += "s";
                    if (connectionmode.HasFlag(ConnectionMode.Exclusive) || connectionmode.HasFlag(ConnectionMode.Private)) mode += "e";
                }
            }
            else
            {
                apiversion = "5";                

                if (connectionmode != ConnectionMode.Default)
                {
                    if (connectionmode.HasFlag(ConnectionMode.ViewOnly)) mode += "v";
                    if (connectionmode.HasFlag(ConnectionMode.Shared)) mode += "s";
                    if (connectionmode.HasFlag(ConnectionMode.Exclusive)) mode += "e";
                    if (connectionmode.HasFlag(ConnectionMode.Private)) mode += "p";
                }

                if (connectionmode == ConnectionMode.Default) mode = "";
            }
          
            string pairs = string.Empty;

            foreach (PresetPair pair in presetpairs)
            {
                if (pairs != string.Empty) pairs += ",";
                pairs += pair.Pair;
            }

            string request = "&method=create_preset&name=" + name;
            if (description != string.Empty) request += "&description=" + description;
            if (pairs != string.Empty) request += "&pairs=" + pairs;
            if (mode != string.Empty) request += "&allowed=" + mode;

            _connect.Get(request, apiversion);
            if (_connect.Response != string.Empty && !_connect.IsError)
            {
                SuccessParser parser = new SuccessParser(_connect.Response);
                return parser.Success;
            }

            return false;
        }


        /// <summary>
        /// Delete a Preset
        /// </summary>
        /// <param name="presetid">Preset Id</param>
        /// <returns>Success</returns>
        public bool DeletePreset(string presetid)
        {
            string apiversion = "3";
            string request = "&method=delete_preset&id=" + presetid;

            _connect.Get(request, apiversion);
            if (_connect.Response != string.Empty && !_connect.IsError)
            {
                SuccessParser parser = new SuccessParser(_connect.Response);
                return parser.Success;
            }

            return false;
        }

        /// <summary>
        /// Create a Channel.  Supported from API Version 4 onwards.
        /// </summary>
        /// <param name="name">Channel Name</param>
        /// <param name="description">Description (Optional, default is empty)</param>
        /// <param name="location">Location (Optional, default is empty)</param>
        /// <param name="connectionmode">Bitwise: Video Only, Shared, Exclusive or Private (Optional, default is global setting)</param>
        /// <param name="video1_id">Device Id (Optional, default is empty)</param>
        /// <param name="video1_head">1 or 2 (Optional, default is 1)</param>
        /// <param name="video2_id">Device Id (Optional, default is empty)</param>
        /// <param name="video2_head">1 or 2 (Optional, default is 1)</param>
        /// <param name="audio_id">Device Id (Optional, default is empty)</param>
        /// <param name="usb_id">Device Id (Optional, default is empty)</param>
        /// <param name="serial_id">Device Id (Optional, default is empty)</param>
        /// <param name="groupname">Group Name (Optional, default is empty)</param>
        /// <returns>Success</returns>
        public bool CreateChannel(string name, string description, string location, ConnectionMode connectionmode, string video1_id, string video1_head, string video2_id, string video2_head, string audio_id, string usb_id, string serial_id, string groupname)
        {
            name = URLSafe(name);
            description = URLSafe(description);
            location = URLSafe(location);
            groupname = URLSafe(groupname);

            if (Useful.Str2Int(APIVersion) >= 4)
            {
                string request = "&method=create_channel&name=" + name;

                if (description != string.Empty) request += "&desc=" + description;
                if (location != string.Empty) request += "&loc=" + location;

                string allowed = "";

                if (connectionmode != ConnectionMode.Default)
                {
                    if (connectionmode.HasFlag(ConnectionMode.ViewOnly)) allowed += "v";
                    if (connectionmode.HasFlag(ConnectionMode.Shared)) allowed += "s";
                    if (connectionmode.HasFlag(ConnectionMode.Exclusive)) allowed += "e";
                    if (connectionmode.HasFlag(ConnectionMode.Private)) allowed += "p";
                    if (allowed != string.Empty) request += "&allowed=" + allowed;
                }

                if (video1_id != string.Empty)
                {
                    request += "&video1=" + video1_id;
                    if (video1_head != string.Empty)
                        request += "&video1head=" + video1_head;
                }

                if (video2_id != string.Empty)
                {
                    request += "&video2=" + video2_id;
                    if (video2_head != string.Empty)
                        request += "&video2head=" + video2_head;
                }

                if (audio_id != string.Empty)
                    request += "&audio=" + audio_id;

                if (usb_id != string.Empty)
                    request += "&usb=" + usb_id;

                if (serial_id != string.Empty)
                    request += "&serial=" + serial_id;

                if (groupname != string.Empty)
                    request += "&groupname=" + groupname;

                _connect.Get(request, "4");

                if (_connect.Response != string.Empty && !_connect.IsError)
                {
                    SuccessParser parser = new SuccessParser(_connect.Response);
                    return parser.Success;
                }
            }

            return false;
        }


        /// <summary>
        /// Delete a Channel. Supported from API Version 4 onwards.
        /// </summary>
        /// <param name="channelid">Channel Id</param>
        /// <returns>Success</returns>
        public bool DeleteChannel(string channelid)
        {
            if (Useful.Str2Int(APIVersion) >= 4)
            {
                string request = "&method=delete_channel&id=" + channelid;

                _connect.Get(request, "4");

                if (_connect.Response != string.Empty && !_connect.IsError)
                {
                    SuccessParser parser = new SuccessParser(_connect.Response);
                    return parser.Success;
                }
            }

            return false;
        }


        /// <summary>
        /// Update Device
        /// </summary>
        /// <param name="deviceid">Device Id</param>
        /// <param name="description">Description</param>
        /// <param name="location">Location</param>
        /// <returns>Success</returns>
        public bool UpdateDevice(string deviceid, string description, string location)
        {
            description = URLSafe(description);
            location = URLSafe(location);

            if (Useful.Str2Int(APIVersion) >= 5)
            {
                string request = "&method=update_device&id=" + deviceid + "&desc=" + description + "&loc=" + location;

                _connect.Get(request, "5");

                if (_connect.Response != string.Empty && !_connect.IsError)
                {
                    SuccessParser parser = new SuccessParser(_connect.Response);
                    return parser.Success;
                }
            }

            return false;
        }


        /// <summary>
        /// Reset Device
        /// </summary>
        /// <param name="deviceid">Device Id</param>
        /// <returns>Success</returns>
        public bool ResetDevices(string deviceid)
        {
            Device device = new Device();
            device.Id = deviceid;
            ResetDevices(deviceid);
            return false;
        }

        /// <summary>
        /// Reset Devices
        /// </summary>
        /// <param name="devices">List of Device Ids</param>
        /// <returns>Success</returns>
        public bool ResetDevices(List<Device> devices)
        {
            if (Useful.Str2Int(APIVersion) >= 5 && devices.Count > 0)
            {
                string ids = string.Empty;

                foreach (Device device in devices)
                {
                    if (ids != string.Empty) ids += ",";
                    ids += device.Id;
                }

                string request = "&method=reboot_devices&ids=" + ids;

                if (ids != string.Empty)
                {
                    _connect.Get(request, "7");

                    if (_connect.Response != string.Empty && !_connect.IsError)
                    {
                        SuccessParser parser = new SuccessParser(_connect.Response);
                        return parser.Success;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Get All Receivers
        /// </summary>
        /// <param name="loglevel">Set Logging Level</param>
        private void GetReceivers()
        {
            GetDevices("&method=get_devices&device_type=rx&results_per_page=" + _pagesize.ToString(), Receivers);
        }


        /// <summary>
        /// Get All Transmitters
        /// </summary>
        /// <param name="loglevel">Set Logging Level</param>
        private void GetTransmitters()
        {
            GetDevices("&method=get_devices&device_type=tx&results_per_page=" + _pagesize.ToString(), Transmitters);
        }

        private void GetDevices(string request, ObservableCollection<Device> devices)
        {
            List<string> xmlpages = GetXMLPages(request, "3");

            if (!_connect.IsError)
            {
                List<Device> tmpdevices = new List<Device>();

                foreach (string xmlpage in xmlpages)
                {
                    DeviceParser parser = new DeviceParser(xmlpage);
                    tmpdevices.AddRange(parser.Devices);
                }

                // Add/Update Device List
                foreach (Device dev in tmpdevices)
                {
                    if (!devices.Contains(dev))
                    {
                        int index = -1;

                        for (int i = 0; i < devices.Count; i++)
                        {
                            if (devices[i].Id == dev.Id) index = i;
                        }

                        if (index > -1)
                            devices[index] = dev;
                        else
                            devices.Add(dev);
                    }
                }

                // Delete from Device List
                int count = 0;
                while (count < devices.Count)
                {
                    if (!tmpdevices.Contains(devices[count])) devices.Remove(devices[count]);
                    count++;
                }
            }
        }


        /// <summary>
        /// Get All Channels
        /// </summary>
        private void GetChannels()
        {
            string request = "&method=get_channels&results_per_page=" + _pagesize.ToString();
            List<string> xmlpages = GetXMLPages(request, "3");

            if (!_connect.IsError)
            {
                List<Channel> tmpChannels = new List<Channel>();

                foreach (string xmlpage in xmlpages)
                {
                    ChannelParser parser = new ChannelParser(xmlpage);
                    tmpChannels.AddRange(parser.Channels);
                }

                // Add/Update Channel List
                foreach (Channel chnl in tmpChannels)
                {
                    if (!Channels.Contains(chnl))
                    {
                        int index = -1;

                        for (int i = 0; i < Channels.Count; i++)
                        {
                            if (Channels[i].Id == chnl.Id) index = i;
                        }

                        if (index > -1)
                            Channels[index] = chnl;
                        else
                            Channels.Add(chnl);
                    }
                }

                // Delete from Channel List
                int count = 0;
                while (count < Channels.Count)
                {
                    if (!tmpChannels.Contains(Channels[count])) Channels.Remove(Channels[count]);
                    count++;
                }
            }
        }


        /// <summary>
        /// Gets and returns a Channel list based on a Receiver Id.  This does not update the main Channels Collection.
        /// </summary>
        /// <param name="receiverid"></param>
        /// <returns>A list of Channels</returns>
        public List<Channel> GetChannelsByReceiverId(string receiverid)
        {
            string request = "&method=get_channels&results_per_page=" + _pagesize.ToString() + "&device_id="+receiverid;
            List<string> xmlpages = GetXMLPages(request, "3");
            List<Channel> channels = new List<Channel>();

            if (!_connect.IsError)
            {
                foreach (string xmlpage in xmlpages)
                {
                    ChannelParser parser = new ChannelParser(xmlpage);
                    channels.AddRange(parser.Channels);
                }
            }

            return channels;
        }


        /// <summary>
        /// Get All Presets
        /// </summary>
        private void GetPresets()
        {
            string request = "&method=get_presets&results_per_page=" + _pagesize.ToString();
            List<string> xmlpages = GetXMLPages(request, "3");

            if (!_connect.IsError)
            {
                List<Preset> tmpPresets = new List<Preset>();

                foreach (string xmlpage in xmlpages)
                {
                    PresetParser parser = new PresetParser(xmlpage);
                    tmpPresets.AddRange(parser.Presets);
                }

                // Add/Update Preset List
                foreach (Preset pres in tmpPresets)
                {
                    if (!Presets.Contains(pres))
                    {
                        int index = -1;

                        for (int i = 0; i < Presets.Count; i++)
                        {
                            if (Presets[i].Id == pres.Id) index = i;
                        }

                        if (index > -1)
                            Presets[index] = pres;
                        else
                            Presets.Add(pres);
                    }
                }

                // Delete from Preset List
                int count = 0;
                while (count < Presets.Count)
                {
                    if (!tmpPresets.Contains(Presets[count])) Presets.Remove(Presets[count]);
                    count++;
                }
            }
        }


        /// <summary>
        /// Get All C-USB
        /// </summary>       
        private void GetCUSBLans()
        {
            string request = "&method=" + _cusbNaming.GetMethod;

            _connect.Get(request, "6");

            if (_connect.Response != string.Empty && !_connect.IsError)
            {
                CUSBParser parser = new CUSBParser(_connect.Response, _cusbNaming);

                List<CUSB> tmpCUSBs = new List<CUSB>();
                tmpCUSBs.AddRange(parser.CUSB);

                // Add/Update CUSB List
                foreach (CUSB cusb in tmpCUSBs)
                {
                    if (!CUSB.Contains(cusb))
                    {
                        int index = -1;

                        for (int i = 0; i < CUSB.Count; i++)
                        {
                            if (CUSB[i].MacAddress == cusb.MacAddress) index = i;
                        }                        

                        if (index > -1)
                            CUSB[index] = cusb;
                        else
                            CUSB.Add(cusb);
                    }
                }               


                // Delete from CUSB List
                int count = 0;
                while (count < CUSB.Count)
                {
                    if (!tmpCUSBs.Contains(CUSB[count])) CUSB.Remove(CUSB[count]);
                    count++;
                }
            }
        }


        /// <summary>
        /// Delete a C-USB LAN. Supported from API Version 6 onwards.
        /// </summary>
        /// <param name="macaddress">MacAddress</param>
        /// <returns>Success</returns>
        public bool DeleteCUSBLan(string macaddress)
        {
            if (Useful.Str2Int(APIVersion) >= 6)
            {
                string request = "&method=" + _cusbNaming.DeleteMethod + "&mac=" + macaddress;

                _connect.Get(request, "6");

                if (_connect.Response != string.Empty && !_connect.IsError)
                {
                    SuccessParser parser = new SuccessParser(_connect.Response);
                    return parser.Success;
                }
            }

            return false;
        }


        /// <summary>
        /// Update a C-USB LAN. Supported from API Version 6 onwards.
        /// </summary>
        /// <param name="macaddress">MacAddress</param>
        /// <param name="name">Name</param>
        /// <returns>Success</returns>
        public bool UpdateCUSBLan(string macaddress, string name)
        {
            name = URLSafe(name);
            
            if (Useful.Str2Int(APIVersion) >= 6)
            {
                string request = "&method="+ _cusbNaming.UpdateMethod +"&mac=" + macaddress + "&name=" + name;

                _connect.Get(request, "6");

                if (_connect.Response != string.Empty && !_connect.IsError)
                {
                    SuccessParser parser = new SuccessParser(_connect.Response);
                    return parser.Success;
                }
            }

            return false;
        }


        /// <summary>
        /// Connect C-USB LAN. Supported from API Version 6 onwards.
        /// </summary>
        /// <param name="rx_macaddress">Receiver MacAddress</param>
        /// <param name="tx_macaddress">Transmitter MacAddress</param>
        /// <returns>Success</returns>
        public bool ConnectCUSBLan(string rx_macaddress, string tx_macaddress)
        {
            if (Useful.Str2Int(APIVersion) >= 6)
            {
                string request = "&method="+ _cusbNaming.ConnectMethod +"&rx=" + rx_macaddress + "&tx=" + tx_macaddress;

                _connect.Get(request, "6");

                if (_connect.Response != string.Empty && !_connect.IsError)
                {
                    SuccessParser parser = new SuccessParser(_connect.Response);
                    return parser.Success;
                }
            }

            return false;
        }

        /// <summary>
        /// Disconnect C-USB LAN. Supported from API Version 6 onwards.
        /// </summary>
        /// <param name="macaddress">MacAddress</param>
        /// <returns>Success</returns>
        public bool DisconnectCUSBLan(string macaddress)
        {
            if (Useful.Str2Int(APIVersion) >= 6)
            {
                string request = "&method=" + _cusbNaming.DisconnectMethod + "&mac=" + macaddress;

                _connect.Get(request, "6");

                if (_connect.Response != string.Empty && !_connect.IsError)
                {
                    SuccessParser parser = new SuccessParser(_connect.Response);
                    return parser.Success;
                }
            }

            return false;
        }

        /// <summary>
        /// Show Results in the Console
        /// </summary>
        public void ShowResults()
        {

            Console.WriteLine("Time: " + DateTime.Now.ToLongTimeString());
            Console.WriteLine("--------------");

            Console.WriteLine("Receivers: " + Receivers.Count.ToString());
            foreach (Device device in Receivers)
            {
                Console.WriteLine("    ID: " + device.Id + " Name: " + device.Name);
            }

            Console.WriteLine("Transmitters: " + Transmitters.Count.ToString());
            foreach (Device device in Transmitters)
            {
                Console.WriteLine("   ID: " + device.Id + " Name: " + device.Name);
            }

            Console.WriteLine("Channels: " + Channels.Count.ToString());
            foreach (Channel channel in Channels)
            {
                Console.WriteLine("   ID: " + channel.Id + " Name: " + channel.Name);
            }

            Console.WriteLine("Presets " + Presets.Count.ToString());
            foreach (Preset preset in Presets)
            {
                Console.WriteLine("   ID: " + preset.Id + " Name: " + preset.Name);
            }
        }

        private List<string> GetXMLPages(string request, string apiversion)
        {
            List<string> xmlpages = new List<string>();
            int page = 1;
            bool complete = false;

            while (!complete)
            {
                _connect.Get(request + "&page=" + page.ToString(), apiversion);
                if (_connect.Response != string.Empty && !_connect.IsError)
                {
                    PageParser parser = new PageParser(_connect.Response, _cusbNaming);
                    if (parser.ItemCount > 0)
                    {
                        xmlpages.Add(_connect.Response);
                        page++;
                    }
                    else complete = true;
                }
                else complete = true;
            }
            return xmlpages;
        }

        public string URLSafe(string value)
        {
            value = value.Replace("%", "%25");
            value = value.Replace(" ", "%20");
            value = value.Replace("$", "%24");
            value = value.Replace("&", "%26");
            value = value.Replace("`", "%60");
            value = value.Replace(":", "%3A");
            value = value.Replace("<", "%3C");
            value = value.Replace(">", "%3E");
            value = value.Replace("[", "%5B");
            value = value.Replace("]", "%5D");
            value = value.Replace("{", "%7B");
            value = value.Replace("}", "%7D");
            value = value.Replace("\"", "%22");
            value = value.Replace("+", "%2B");
            value = value.Replace("#", "%23");
            value = value.Replace("@", "%40");
            value = value.Replace("/", "%2F");
            value = value.Replace(";", "%3B");
            value = value.Replace("=", "%3D");
            value = value.Replace("?", "%3F");
            value = value.Replace("\\", "%5C");
            value = value.Replace("^", "%5E");
            value = value.Replace("|", "%7C");
            value = value.Replace("~", "%7E");
            value = value.Replace("'", "%27");
            value = value.Replace(",", "%2C");
            return value;
        }

        #region Events

        private void CollectionChangedEvent(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_context != null)
                Synchronise(this.CollectionChangedEventMainThread, e);
            else
                CollectionChanged(e);
        }

        private void CollectionChangedEventMainThread(object obj)
        {
            if (obj != null)
            {
                NotifyCollectionChangedEventArgs e = (NotifyCollectionChangedEventArgs)obj;
                CollectionChanged(e);
            }
        }

        private void CollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            //Add
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                {
                    foreach (Object item in e.NewItems)
                    {
                        if (item.GetType() == typeof(Device))
                        {
                            Device device = (Device)item;
                            if (device.Type == DeviceType.RX)
                            {
                                if (OnReceiverAdded != null) OnReceiverAdded(device, new EventArgs());
                            }
                            else
                            {
                                if (OnTransmitterAdded != null) OnTransmitterAdded(device, new EventArgs());
                            }
                        }
                        if (item.GetType() == typeof(Channel))
                        {
                            Channel channel = (Channel)item;
                            if (OnChannelAdded != null) OnChannelAdded(channel, new EventArgs());
                        }
                        if (item.GetType() == typeof(Preset))
                        {
                            Preset preset = (Preset)item;
                            if (OnPresetAdded != null) OnPresetAdded(preset, new EventArgs());
                        }
                        if (item.GetType() == typeof(CUSB))
                        {
                            CUSB cusb = (CUSB)item;
                            if (OnCUSBAdded != null) OnCUSBAdded(cusb, new EventArgs());
                        }
                    }
                }
            }

            //Remove
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    foreach (Object item in e.OldItems)
                    {
                        if (item.GetType() == typeof(Device))
                        {
                            Device device = (Device)item;
                            if (device.Type == DeviceType.RX)
                            {
                                if (OnReceiverRemoved != null) OnReceiverRemoved(device, new EventArgs());
                            }
                            else
                            {
                                if (OnTransmitterRemoved != null) OnTransmitterRemoved(device, new EventArgs());
                            }
                        }
                        if (item.GetType() == typeof(Channel))
                        {
                            Channel channel = (Channel)item;
                            if (OnChannelRemoved != null) OnChannelRemoved(channel, new EventArgs());
                        }
                        if (item.GetType() == typeof(Preset))
                        {
                            Preset preset = (Preset)item;
                            if (OnPresetRemoved != null) OnPresetRemoved(preset, new EventArgs());
                        }
                        if (item.GetType() == typeof(CUSB))
                        {
                            CUSB cusb = (CUSB)item;
                            if (OnCUSBRemoved != null) OnCUSBRemoved(cusb, new EventArgs());
                        }
                    }
                }
            }

            //Updates
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.NewItems != null)
                {
                    foreach (Object item in e.NewItems)
                    {
                        if (item.GetType() == typeof(Device))
                        {
                            Device device = (Device)item;
                            if (device.Type == DeviceType.RX)
                            {
                                if (OnReceiverUpdated != null) OnReceiverUpdated(device, new EventArgs());
                            }
                            else
                            {
                                if (OnTransmitterUpdated != null) OnTransmitterUpdated(device, new EventArgs());
                            }
                        }
                        if (item.GetType() == typeof(Channel))
                        {
                            Channel channel = (Channel)item;
                            if (OnChannelUpdated != null) OnChannelUpdated(channel, new EventArgs());
                        }
                        if (item.GetType() == typeof(Preset))
                        {
                            Preset preset = (Preset)item;
                            if (OnPresetUpdated != null) OnPresetUpdated(preset, new EventArgs());
                        }
                        if (item.GetType() == typeof(CUSB))
                        {
                            CUSB cusb = (CUSB)item;
                            if (OnCUSBUpdated != null) OnCUSBUpdated(cusb, new EventArgs());
                        }
                    }
                }
            }

            if (OnUpdate != null) OnUpdate(this, e);
        }

        #region ErrorEvent
        private void ErrorEvent(Object sender, EventArgs e)
        {
            Disconnect();

            if (_context != null)
                Synchronise(this.ErrorEventMainThread, e);
            else
                if (OnError != null) OnError(sender, e);
        }

        private void ErrorEventMainThread(object obj)
        {
            if (OnError != null) OnError(this, (EventArgs)obj);
        }
        #endregion

        #region ConnectingEvent

        private void ConnectingEvent(Object sender, EventArgs e)
        {
            lock (this)
            {
                if (_context != null)
                    Synchronise(this.ConnectingEventMainThread, e);
                else
                    if (OnConnecting != null) OnConnecting(sender, e);
            }
        }

        private void ConnectingEventMainThread(object obj)
        {
            if (OnConnecting != null) OnConnecting(this, (EventArgs)obj);
        }
        #endregion

        private void ConnectedEvent(Object sender, EventArgs e)
        {
            lock (this)
            {
                if (_context != null)
                    Synchronise(this.ConnectedEventMainThread, e);
                else
                {
                    if (OnConnected != null) OnConnected(sender, e);
                }
            }
        }

        private void ConnectedEventMainThread(object obj)
        {
            if (OnConnected != null) OnConnected(this, (EventArgs)obj);
        }

        private void DisconnectedEventMainThread(object obj)
        {
            if (OnDisconnected != null) OnDisconnected(this, (EventArgs)obj);
        }

        private void DisconnectedEvent(Object sender, EventArgs e)
        {
            lock (this)
            {
                if (_context != null)
                    Synchronise(this.DisconnectedEventMainThread, e);
                else
                {
                    if (OnDisconnected != null) OnDisconnected(sender, e);
                }
            }
        }

        private void TokenChangedEvent(Object sender, EventArgs e)
        {
            lock (this)
            {
                if (_context != null)
                    Synchronise(this.TokenChangedEventMainThread, e);
                else
                {
                    if (OnTokenChanged != null) OnTokenChanged(sender, e);
                }
            }
        }

        private void TokenChangedEventMainThread(object obj)
        {
            if (OnTokenChanged != null) OnTokenChanged(this, (EventArgs)obj);
        }

        private void CompleteEvent(Object sender, EventArgs e)
        {
            lock (this)
            {
                if (_context != null)
                    Synchronise(this.CompleteEventMainThread, e);
                else
                    if (OnComplete != null) OnComplete(this, e);
            }
        }

        private void CompleteEventMainThread(object obj)
        {
            if (OnComplete != null) OnComplete(this, (EventArgs)obj);
        }

        private void Synchronise(SendOrPostCallback handler, object argument)
        {
            if (_context != null)
                _context.Post(handler, argument);
        }


        private void OnTimerGetUpdate(object sender, ElapsedEventArgs e)
        {
            Get();
        }

        #endregion

        private void DebugLog(string message)
        {
            if (Debug) Console.WriteLine(DateTime.Now.ToShortTimeString() + ":  AIMAPI: " + message);
        }
    }
}
