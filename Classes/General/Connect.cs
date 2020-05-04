using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using AIMAPI.Classes.Parsers;
using AIMAPI.Classes.General;

namespace AIMAPI.General
{
    public class Connect
    {
        #region Events
        public event EventHandler OnConnecting;
        public event EventHandler OnConnected;
        public event EventHandler OnResponse;
        public event EventHandler OnError;
        public event EventHandler OnTokenChanged;
        #endregion

        const int TIMEOUT = 5000;

        private string _ipaddress = string.Empty;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _token = string.Empty;
        private string _response = string.Empty;
        private string _errorcode = string.Empty;
        private string _errormsg = string.Empty;
        private string _version = string.Empty;
        private int _retries = 0;
        private int _httpport = 80;
        private bool _disconnect = true;
        private bool _ssl = false;
        private bool _tokenrequired = true;
        private bool _error = false;

        public string IP
        {
            get { return _ipaddress; }
            set 
            {
                if (value != _ipaddress) _token = string.Empty;
                _ipaddress = value; 
            }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }

        public bool TokenRequired
        {
            get { return _tokenrequired; }
            set { _tokenrequired = value; }
        }

        public string Response
        {
            get { return _response; }
        }

        public int Retries
        {
            get { return _retries; }
            set { _retries = value; }
        }

        public bool IsError
        {
            get { return _error; }
            set { _error = value; }
        }

        public bool IsDisconnected
        {
            get { return _disconnect; }
            set { _disconnect = value; }
        }

        public bool Abort
        {
            get { if (_error || _disconnect) return true; else return false; }
        }

        public string Version
        {
            get { return _version; }
        }

        public string ErrorCode
        {
            get { return _errorcode; }
        }

        public string ErrorMessage
        {
            get { return _errormsg; }
        }

        public int HTTPPort
        {
            get { return _httpport; }
            set { _httpport = value; }
        }

        public bool UseSSL
        {
            get { return _ssl; }
            set { _ssl = value; }
        }

        public void Disconnect()
        {
            _disconnect = true;
        }

        public Connect() { }

        public Connect(string ipaddress, string username, string password, bool ssl)
        {
            _ipaddress = ipaddress;
            _username = username;
            _password = password;
            _ssl = ssl;
        }

        public void Get(string parameters, string apiversion)
        {
            _disconnect = false;            

            string response = GetResponse(parameters, apiversion);

            if (response != string.Empty)
            {
                SuccessParser success = new SuccessParser(response);
                if (!success.Success)
                {
                    //Not logged in
                    if (success.ErrorCode == "10")
                    {
                        response = string.Empty;

                        if (Login())
                        {
                            response = GetResponse(parameters, apiversion);
                            success = new SuccessParser(response);
                        }
                    }
                    if (success.ErrorCode != string.Empty)
                    {
                        _errorcode = success.ErrorCode;
                        _errormsg = success.ErrorMessage;
                        _error = true;
                    }
                }

                if (response != string.Empty)
                {                    
                    _response = response;
                    _version = success.Version;
                    if (OnResponse != null) OnResponse(this, new EventArgs());
                }
            }
            else
            {
                _errorcode = "1";
                _errormsg = "Unable to Connect to AIM Server";
            }
   
        }

        private string GetResponse(string parameters, string apiversion)
        {
            string port = "";                        

            IPAddress ipaddress;
            if (IPAddress.TryParse(_ipaddress, out ipaddress) == false) throw new System.ArgumentException("Invalid IP address");

            if (_httpport != 80 && _httpport != 443)
            {
                port = ":" + _httpport.ToString();
            }

            string protocolType = "http";
            if (_ssl)
            {
                protocolType = "https";
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }; 
            }

            // e.g. htttp//<IPADDRESS>:<PORT>/api/?v=
            String url = protocolType + "://" + _ipaddress + port + "/api/?v=" + apiversion + parameters;
            if (_tokenrequired && url.IndexOf("&method=login") == -1 && _token != string.Empty) url = url + "&token=" + _token;

            for (int loop = 0; loop <= _retries; loop++)
            {
                if (!_disconnect)
                {
                    try
                    {
                        if (OnConnecting != null) OnConnecting(this, new EventArgs());
                        WebRequest webrequest = WebRequest.Create(url);
                        webrequest.Proxy = null;
                        webrequest.Timeout = TIMEOUT;
                        WebResponse webresponse = webrequest.GetResponse();

                        string response = "";

                        using (StreamReader sr = new StreamReader(webresponse.GetResponseStream()))
                        {
                            if (OnConnected != null) OnConnected(this, new EventArgs());
                            response = sr.ReadToEnd();
                            sr.Close();
                        }
                        webresponse.Close();
                        return response;
                    }

                    catch (WebException e)
                    {
                        if (loop == _retries)
                        {
                            _errorcode = "999";
                            _errormsg = "Failed to Connect to Server";
                            _error = true;
                            if (OnError != null) OnError(this, new EventArgs());
                        }
                    }
                }
            }

            return "";
        }

        public bool Login(string username, string password)
        {
            _username = username;
            _password = password;
            return Login();
        }

        private bool Login()
        {
            Boolean result = false;
            String url = "&method=login&username=" + _username;

            if (_password != string.Empty) url += "&password=" + _password;
            string response = GetResponse(url, "3");

            SuccessParser success = new SuccessParser(response);
            if (success.Success)
            {
                LoginParser login = new LoginParser(response);

                if (login.Success)
                {
                    if (_token != login.Token)
                    {
                        if (OnTokenChanged != null) OnTokenChanged(this, new EventArgs());
                    }
                    _token = login.Token;
                    result = true;
                }
            }
            else
            {
                _errorcode = success.ErrorCode;
                _errormsg = success.ErrorMessage;
                _error = true;
                if (OnError != null) OnError(this, new EventArgs());
            }

            return result;
        }     
    }
}
