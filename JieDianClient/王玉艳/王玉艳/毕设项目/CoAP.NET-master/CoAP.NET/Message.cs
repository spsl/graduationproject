﻿/*
 * Copyright (c) 2011-2013, Longxiang He <helongxiang@smeshlink.com>,
 * SmeshLink Technology Co.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY.
 * 
 * This file is part of the CoAP.NET, a CoAP framework in C#.
 * Please see README for more information.
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using CoAP.Layers;
using CoAP.Log;
using CoAP.Util;

namespace CoAP
{
    /// <summary>
    /// This class describes the functionality of a CoAP message.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Invalid message ID.
        /// </summary>
        public const Int32 InvalidID = -1;

        private static readonly ILogger log = LogManager.GetLogger(typeof(Message));

        private Int32 _version = Spec.SupportedVersion;
        private MessageType _type;
        private Int32 _code;
        private Int32 _id = InvalidID;
        private Byte[] _token;
        private Byte[] _payLoadBytes;
        private Boolean _requiresToken = true;
        private Boolean _requiresBlockwise = false;
        private SortedDictionary<OptionType, IList<Option>> _optionMap = new SortedDictionary<OptionType, IList<Option>>();
        private Int64 _timestamp;
        private Uri _uri;
        private Int32 _retransmissioned;
        private Int32 _maxRetransmit = CoapConstants.MaxRetransmit;
        private Int32 _responseTimeout = CoapConstants.ResponseTimeout;
        private EndpointAddress _peerAddress;
        private Boolean _cancelled = false;
        private Boolean _complete = false;
        private ILayer _communicator;

        /// <summary>
        /// Initializes a message.
        /// </summary>
        public Message()
        {
        }

        /// <summary>
        /// Initializes a message.
        /// </summary>
        /// <param name="type">The message type</param>
        /// <param name="code">The message code</param>
        public Message(MessageType type, Int32 code)
        {
            this._type = type;
            this._code = code;
        }

        /// <summary>
        /// Initializes a message.
        /// </summary>
        public Message(Uri uri, MessageType type, Int32 code, Int32 id, Byte[] payload)
        {
            URI = uri;
            _type = type;
            _code = code;
            _id = id;
            _payLoadBytes = payload;
        }

        /// <summary>
        /// Creates a reply message to this message, which addressed to the
        /// peer and has the same message ID and token.
        /// </summary>
        /// <param name="ack">Acknowledgement or not</param>
        /// <returns></returns>
        public Message NewReply(Boolean ack)
        {
            Message reply = new Message();

            if (this._type == MessageType.CON)
            {
                reply._type = ack ? MessageType.ACK : MessageType.RST;
            }
            else
            {
                reply._type = MessageType.NON;
            }

            // echo ID
            reply._id = this._id;
            // set the receiver URI of the reply to the sender of this message
            reply._peerAddress = _peerAddress;

            // echo token
            reply.Token = Token;
            reply.RequiresToken = this.RequiresToken;
            // create an empty reply by default
            reply.Code = CoAP.Code.Empty;

            return reply;
        }

        /// <summary>
        /// Creates a new ACK message with peer address and MID matching to this message.
        /// </summary>
        public Message NewAccept()
        {
            Message ack = new Message(MessageType.ACK, CoAP.Code.Empty);
            ack.PeerAddress = this.PeerAddress;
            ack.ID = this.ID;
            // echo token
            ack.Token = Token;
            return ack;
        }

        /// <summary>
        /// Creates a new RST message with peer address and MID matching to this message.
        /// </summary>
        public Message NewReject()
        {
            Message rst = new Message(MessageType.RST, CoAP.Code.Empty);
            rst.PeerAddress = this.PeerAddress;
            rst.ID = this.ID;
            return rst;
        }

        /// <summary>
        /// Accepts this message with an empty ACK. Use this method only at
        /// application level, as the ACK will be sent through the whole stack.
        /// Within the stack use NewAccept() and send it through the corresponding lower layer.
        /// </summary>
        public virtual void Accept()
        {
            if (IsConfirmable)
            {
                Message msg = NewAccept();
                msg.Communicator = this.Communicator;
                msg.Send();
            }
        }

        /// <summary>
        /// Rejects this message with an empty RST. Use this method only at
        /// application level, as the RST will be sent through the whole stack.
        /// Within the stack use NewReject() and send it through the corresponding lower layer.
        /// </summary>
        public void Reject()
        {
            Message msg = NewReject();
            msg.Communicator = this.Communicator;
            msg.Send();
        }

        /// <summary>
        /// Sends this message.
        /// </summary>
        public void Send()
        {
            Communicator.SendMessage(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public void HandleBy(IMessageHandler handler)
        {
            DoHandleBy(handler);
        }

        /// <summary>
        /// Notification method that is called when the transmission of this
        /// message was canceled due to timeout.
        /// </summary>
        public void HandleTimeout()
        {
            DoHandleTimeout();
        }

        /// <summary>
        /// Appends data to this message's payload.
        /// </summary>
        /// <param name="block">The byte array to be appended</param>
        public void AppendPayload(Byte[] block)
        {
            if (null != block)
            {
                System.Threading.Monitor.Enter(this);
                if (null == this._payLoadBytes)
                {
                    this._payLoadBytes = (Byte[])block.Clone();
                }
                else
                {
                    Byte[] newPayload = new Byte[this._payLoadBytes.Length + block.Length];
                    Array.Copy(this._payLoadBytes, 0, newPayload, 0, this._payLoadBytes.Length);
                    Array.Copy(block, 0, newPayload, this._payLoadBytes.Length, block.Length);
                    this._payLoadBytes = newPayload;
                }
                System.Threading.Monitor.PulseAll(this);
                // TODO add event
                PayloadAppended(block);
                System.Threading.Monitor.Exit(this);
            }
        }

        /// <summary>
        /// Sets the payload of this CoAP message.
        /// </summary>
        /// <param name="payload">The string representation of the payload</param>
        public void SetPayload(String payload)
        {
            if (payload == null)
                payload = String.Empty;
            Payload = System.Text.Encoding.UTF8.GetBytes(payload);
        }

        /// <summary>
        /// Sets the payload of this CoAP message.
        /// </summary>
        /// <param name="payload">The string representation of the payload</param>
        /// <param name="mediaType">The content-type of the payload</param>
        public void SetPayload(String payload, Int32 mediaType)
        {
            if (payload == null)
                payload = String.Empty;
            Payload = System.Text.Encoding.UTF8.GetBytes(payload);
            ContentType = mediaType;
        }

        /// <summary>
        /// To string.
        /// </summary>
        public override String ToString()
        {
#if DEBUG
            StringBuilder builder = new StringBuilder();
            String kind = "MESSAGE";
            if (this.IsRequest)
                kind = "REQUEST";
            else if (this.IsResponse)
                kind = "RESPONSE";
            builder.AppendFormat("==[ COAP {0} ]============================================\n", kind);

            IList<Option> options = GetOptions();
            builder.AppendFormat("Address:  {0}\n", PeerAddress == null ? "local" : PeerAddress.ToString());
            builder.AppendFormat("ID     :  {0}\n", _id);
            builder.AppendFormat("Type   :  {0}\n", Type);
            builder.AppendFormat("Code   :  {0}\n", CoAP.Code.ToString(_code));
            builder.AppendFormat("Options:  {0}\n", options.Count);
            foreach (Option opt in options)
            {
                builder.AppendFormat("  * {0}: {1} ({2} Bytes)\n", opt.Name, opt, opt.Length);
            }
            builder.AppendFormat("Payload: {0} Bytes\n", this.PayloadSize);
            if (this.PayloadSize > 0)
            {
                builder.AppendLine("---------------------------------------------------------------");
                builder.AppendLine(this.PayloadString);
            }
            builder.AppendLine("===============================================================");

            return builder.ToString();
#else
            return String.Format("{0}: [{1}] {2} '{3}'({4})",
                Key, Type, CoAP.Code.ToString(_code),
                PayloadString, PayloadSize);
#endif
        }

        /// <summary>
        /// Equals.
        /// </summary>
        public override Boolean Equals(Object obj)
        {
            if (obj == null)
                return false;
            if (Object.ReferenceEquals(this, obj))
                return true;
            if (this.GetType() != obj.GetType())
                return false;
            Message other = (Message)obj;
            if (_type != other._type)
                return false;
            if (_version != other._version)
                return false;
            if (_code != other._code)
                return false;
            if (_id != other._id)
                return false;
            if (_optionMap == null)
            {
                if (other._optionMap != null)
                    return false;
            }
            else if (!_optionMap.Equals(other._optionMap))
                return false;
            if (!Sort.IsSequenceEqualTo(_payLoadBytes, other._payLoadBytes))
                return false;
            if (PeerAddress == null)
            {
                if (other.PeerAddress != null)
                    return false;
            }
            else if (!PeerAddress.Equals(other.PeerAddress))
                return false;
            return true;
        }

        /// <summary>
        /// Get hash code.
        /// </summary>
        public override Int32 GetHashCode()
        {
            return base.GetHashCode();
        }

        #region Option operations

        /// <summary>
        /// Adds an option to the list of options of this CoAP message.
        /// </summary>
        /// <param name="option">the option to add</param>
        public void AddOption(Option option)
        {
            if (option == null)
                throw new ArgumentNullException("opt");

            IList<Option> list = null;
            if (_optionMap.ContainsKey(option.Type))
                list = _optionMap[option.Type];
            else
            {
                list = new List<Option>();
                _optionMap[option.Type] = list;
            }

            list.Add(option);

            if (option.Type == OptionType.Token)
                Token = option.RawValue;
        }

        /// <summary>
        /// Adds all option to the list of options of this CoAP message.
        /// </summary>
        /// <param name="options">the options to add</param>
        public void AddOptions(IEnumerable<Option> options)
        {
            foreach (Option opt in options)
            {
                AddOption(opt);
            }
        }

        /// <summary>
        /// Removes all options of the given type from this CoAP message.
        /// </summary>
        /// <param name="optionType">the type of option to remove</param>
        public void RemoveOptions(OptionType optionType)
        {
            _optionMap.Remove(optionType);
        }

        /// <summary>
        /// Gets all options of the given type.
        /// </summary>
        /// <param name="optionType">the option type</param>
        /// <returns></returns>
        public IEnumerable<Option> GetOptions(OptionType optionType)
        {
            return _optionMap.ContainsKey(optionType) ? _optionMap[optionType] : null;
        }

        /// <summary>
        /// Sets an option.
        /// </summary>
        /// <param name="opt">the option to set</param>
        public void SetOption(Option opt)
        {
            if (null != opt)
            {
                RemoveOptions(opt.Type);
                AddOption(opt);
            }
        }

        /// <summary>
        /// Sets all options with the specified option type.
        /// </summary>
        /// <param name="options">the options to set</param>
        public void SetOptions(IEnumerable<Option> options)
        {
            if (options == null)
                return;
            foreach (Option opt in options)
            {
                RemoveOptions(opt.Type);
            }
            AddOptions(options);
        }

        /// <summary>
        /// Checks if this CoAP message has options of the specified option type.
        /// </summary>
        /// <param name="type">the option type</param>
        /// <returns>rrue if options of the specified type exist</returns>
        public Boolean HasOption(OptionType type)
        {
            return GetFirstOption(type) != null;
        }

        /// <summary>
        /// Gets the first option of the specified option type.
        /// </summary>
        /// <param name="optionType">the option type</param>
        /// <returns>the first option of the specified type, or null</returns>
        public Option GetFirstOption(OptionType optionType)
        {
            IList<Option> list = _optionMap.ContainsKey(optionType) ? _optionMap[optionType] : null;
            return (null != list && list.Count > 0) ? list[0] : null;
        }

        /// <summary>
        /// Gets a sorted list of all options.
        /// </summary>
        /// <returns></returns>
        public IList<Option> GetOptions()
        {
            List<Option> list = new List<Option>();
            foreach (IList<Option> opts in this._optionMap.Values)
            {
                if (null != opts)
                    list.AddRange(opts);
            }
            return list;
        }

        /// <summary>
        /// Gets the number of all options of this CoAP message.
        /// </summary>
        /// <returns></returns>
        public Int32 GetOptionCount()
        {
            return GetOptions().Count;
        }

        #endregion

        #region Properties

        public ILayer Communicator
        {
            get
            {
                if (_communicator == null)
                    _communicator = CoAP.Communicator.Default;
                return _communicator;
            }
            set { _communicator = value; }
        }

        /// <summary>
        /// Gets the size of the payload of this CoAP message.
        /// </summary>
        public Int32 PayloadSize
        {
            get { return (null == this._payLoadBytes) ? 0 : this._payLoadBytes.Length; }
        }

        /// <summary>
        /// Gets or sets the payload of this CoAP message.
        /// </summary>
        public Byte[] Payload
        {
            get { return this._payLoadBytes; }
            set { this._payLoadBytes = value; }
        }

        /// <summary>
        /// Gets or sets the payload of this CoAP message in string representation.
        /// </summary>
        public String PayloadString
        {
            get { return (null == this._payLoadBytes) ? null : System.Text.Encoding.UTF8.GetString(this._payLoadBytes); }
            set { SetPayload(value, MediaType.TextPlain); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether a generated token is needed.
        /// </summary>
        public Boolean RequiresToken
        {
            get { return _requiresToken && Code != CoAP.Code.Empty; }
            set { _requiresToken = value; }
        }

        public Boolean RequiresBlockwise
        {
            get { return this._requiresBlockwise; }
            set { this._requiresBlockwise = value; }
        }

        /// <summary>
        /// Gets or sets the timestamp related to this CoAP message.
        /// </summary>
        public Int64 Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        /// <summary>
        /// Gets or sets how many times this message has been retransmissioned.
        /// </summary>
        public Int32 Retransmissioned
        {
            get { return _retransmissioned; }
            set { _retransmissioned = value; }
        }

        /// <summary>
        /// Gets or sets the max times this message should be retransmissioned.
        /// By default the value is equal to <code>CoapConstants.MaxRetransmit</code>.
        /// A value of 0 indicates that this message will not be retransmissioned when timeout.
        /// </summary>
        public Int32 MaxRetransmit
        {
            get { return _maxRetransmit; }
            set { _maxRetransmit = value; }
        }

        /// <summary>
        /// Gets or sets the amount of time in milliseconds after which this message will time out.
        /// The default value is <code>CoapConstants.ResponseTimeout</code>.
        /// A value less or equal than 0 indicates an infinite time-out period.
        /// </summary>
        public Int32 ResponseTimeout
        {
            get { return _responseTimeout; }
            set { _responseTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the URI of this CoAP message.
        /// </summary>
        public Uri URI
        {
            get { return this._uri; }
            set
            {
                if (null != value)
                {
                    // TODO Uri-Host option

                    UriPath = value.AbsolutePath;
                    UriQuery = value.Query;
                    PeerAddress = new EndpointAddress(value);
                }
                this._uri = value;
            }
        }

        public String UriHost
        {
            get
            {
                Option host = GetFirstOption(OptionType.UriHost);
                if (host == null)
                {
                    if (_peerAddress != null && _peerAddress.Address != null)
                    {
                        return _peerAddress.Address.ToString();
                    }
                    else
                        return "localhost";
                }
                else
                {
                    return host.StringValue;
                }
            }
        }

        public String UriPath
        {
            get { return "/" + Option.Join(GetOptions(OptionType.UriPath), "/"); }
            set { SetOptions(Option.Split(OptionType.UriPath, value, "/")); }
        }

        public String UriQuery
        {
            get { return Option.Join(GetOptions(OptionType.UriQuery), "&"); }
            set
            {
                if (!String.IsNullOrEmpty(value) && value.StartsWith("?"))
                    value = value.Substring(1);
                SetOptions(Option.Split(OptionType.UriQuery, value, "&"));
            }
        }

        public Byte[] Token
        {
            get { return _token == null ? TokenManager.EmptyToken : _token; }
            set
            {
                if (value != null && value != TokenManager.EmptyToken)
                    _token = (Byte[])value.Clone();
                RequiresToken = false;

                // for compatibility with CoAP 13-
                List<Option> list = new List<Option>(1);
                list.Add(Option.Create(OptionType.Token, value));
                _optionMap[OptionType.Token] = list;
            }
        }

        public String TokenString
        {
            get { return ByteArrayUtils.ToHexString(Token); }
        }

        /// <summary>
        /// Gets or sets the content-type of this CoAP message.
        /// </summary>
        public Int32 ContentType
        {
            get
            {
                Option opt = GetFirstOption(OptionType.ContentType);
                return (null == opt) ? MediaType.Undefined : opt.IntValue;
            }
            set
            {
                if (value == MediaType.Undefined)
                {
                    RemoveOptions(OptionType.ContentType);
                }
                else
                {
                    SetOption(Option.Create(OptionType.ContentType, value));
                }
            }
        }

        public Int32 FirstAccept
        {
            get {
                Option opt = GetFirstOption(OptionType.Accept);
                return opt == null ? MediaType.Undefined : opt.IntValue;
            }
        }

        /// <summary>
        /// Gets or set the location-path of this CoAP message.
        /// </summary>
        public String LocationPath
        {
            get
            {
                return Option.Join(GetOptions(OptionType.LocationPath), "/");
            }
            set
            {
                SetOptions(Option.Split(OptionType.LocationPath, value, "/"));
            }
        }

        public String LocationQuery
        {
            get { return Option.Join(GetOptions(OptionType.LocationQuery), "&"); }
            set
            {
                if (!String.IsNullOrEmpty(value) && value.StartsWith("?"))
                    value = value.Substring(1);
                SetOptions(Option.Split(OptionType.LocationQuery, value, "&"));
            }
        }

        /// <summary>
        /// Gets or sets the max-age of this CoAP message.
        /// </summary>
        public Int32 MaxAge
        {
            get
            {
                Option opt = GetFirstOption(OptionType.MaxAge);
                return (null == opt) ? CoapConstants.DefaultMaxAge : opt.IntValue;
            }
            set
            {
                SetOption(Option.Create(OptionType.MaxAge, value));
            }
        }

        /// <summary>
        /// Gets or sets the code of this CoAP message.
        /// </summary>
        public Int32 Code
        {
            get { return this._code; }
            set { this._code = value; }
        }

        /// <summary>
        /// Gets the code's string representation of this CoAP message.
        /// </summary>
        public String CodeString
        {
            get { return CoAP.Code.ToString(this._code); }
        }

        /// <summary>
        /// Gets the version of this CoAP message.
        /// </summary>
        public Int32 Version
        {
            get { return _version; }
        }

        /// <summary>
        /// Gets or sets the ID of this CoAP message.
        /// </summary>
        public Int32 ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether this CoAP message is canceled.
        /// </summary>
        public Boolean Canceled
        {
            get { return _cancelled; }
            set { _cancelled = value; }
        }

        /// <summary>
        /// Gets or sets the type of this CoAP message.
        /// </summary>
        public MessageType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Gets a value that indicates whether this CoAP message is a request message.
        /// </summary>
        public Boolean IsRequest
        {
            get { return CoAP.Code.IsRequest(_code); }
        }

        /// <summary>
        /// Gets a value that indicates whether this CoAP message is a response message.
        /// </summary>
        public Boolean IsResponse
        {
            get { return CoAP.Code.IsResponse(_code); }
        }

        /// <summary>
        /// Gets a value that indicates whether this CoAP message is confirmable.
        /// </summary>
        public Boolean IsConfirmable
        {
            get { return _type == MessageType.CON; }
        }

        /// <summary>
        /// Gets a value that indicates whether this CoAP message is non-confirmable.
        /// </summary>
        public Boolean IsNonConfirmable
        {
            get { return _type == MessageType.NON; }
        }

        /// <summary>
        /// Gets a value that indicates whether this CoAP message is an acknowledgement.
        /// </summary>
        public Boolean IsAcknowledgement
        {
            get { return _type == MessageType.ACK; }
        }

        /// <summary>
        /// Gets a value that indicates whether this CoAP message is a reset.
        /// </summary>
        public Boolean IsReset
        {
            get { return _type == MessageType.RST; }
        }

        /// <summary>
        /// Gets a value that indicates whether this CoAP message is an reply message.
        /// </summary>
        public Boolean IsReply
        {
            get { return IsAcknowledgement || IsReset; }
        }

        /// <summary>
        /// Gets a value that indicates whether this response is a separate one.
        /// </summary>
        public Boolean IsEmptyACK
        {
            get { return IsAcknowledgement && Code == CoAP.Code.Empty; }
        }

        /// <summary>
        /// Gets a string that is assumed to uniquely identify a message,
        /// since messages from different remote endpoints might have a same message ID.
        /// </summary>
        public String Key
        {
            get
            {
                return String.Format("{0}|{1}|{2}", PeerAddress == null ? "local" : PeerAddress.ToString(), _id, Type);
            }
        }

        public String TransactionKey
        {
            get
            {
                return String.Format("{0}|{1}", PeerAddress == null ? "local" : PeerAddress.ToString(), _id);
            }
        }

        public String SequenceKey
        {
            get
            {
                return String.Format("{0}#{1}", PeerAddress == null ? "local" : PeerAddress.ToString(), TokenString);
            }
        }

        public Int32 Port
        {
            get { return null == this._uri ? CoapConstants.DefaultPort : this._uri.Port; }
        }

        /// <summary>
        /// Gets the IP address of the URI of this CoAP message.
        /// </summary>
        public IPAddress Address
        {
            get
            {
                if (null == this._uri)
                    return null;
                else
                {
                    IPAddress[] addrs = Dns.GetHostAddresses(this._uri.Host);
                    return addrs[0];
                }
            }
        }

        public EndpointAddress PeerAddress
        {
            get { return _peerAddress; }
            set { _peerAddress = value; }
        }

        /// <summary>
        /// Gets the endpoint ID of this CoAP message, including ip address and port.
        /// </summary>
        public String EndPointID
        {
            get
            {
                IPAddress addr = Address;

                // TODO 检查IP地址是否需要[]
                return String.Format("[{0}]:{1}", addr, Port);
            }
        }

        #endregion

        /// <summary>
        /// Notification method that is called when the transmission of this
        /// message was canceled due to timeout.
        /// <remarks>Subclasses may override this method to add custom handling code.</remarks>
        /// </summary>
        protected virtual void DoHandleTimeout()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        protected virtual void DoHandleBy(IMessageHandler handler)
        { }

        /// <summary>
        /// Creates a message with subtype according to code number
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Message Create(Int32 code)
        {
            if (CoAP.Code.IsRequest(code))
            {
                switch (code)
                {
                    case CoAP.Code.GET:
                        return new GETRequest();
                    case CoAP.Code.POST:
                        return new POSTRequest();
                    case CoAP.Code.PUT:
                        return new PUTRequest();
                    case CoAP.Code.DELETE:
                        return new DELETERequest();
                    default:
                        return new UnsupportedRequest(code);
                }
            }
            else if (CoAP.Code.IsResponse(code))
            {
                return new Response(code);
            }
            else if (code == CoAP.Code.Empty)
            {
                // empty messages are handled as responses
                // in order to handle ACK/RST messages consistent
                // with actual responses
                return new Response(code);
            }
            else
            {
                return new Message(MessageType.CON, code);
            }
        }

        protected virtual void PayloadAppended(Byte[] block)
        { }
    }
}
