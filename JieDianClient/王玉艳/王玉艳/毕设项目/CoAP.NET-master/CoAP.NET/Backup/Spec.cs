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
using CoAP.Log;
using CoAP.Util;

namespace CoAP
{
#if COAPALL
    public static class Spec
    {
        public const Int32 SupportedVersion = 1;
        public const Int32 DefaultPort = 5683;
        public static readonly ISpec Draft03 = new CoAP.Draft03();
        public static readonly ISpec Draft08 = new CoAP.Draft08();
        public static readonly ISpec Draft12 = new CoAP.Draft12();
        public static readonly ISpec Draft13 = new CoAP.Draft13();
    }

    public interface ISpec
    {
        String Name { get; }
        Int32 SupportedVersion { get; }
        Int32 DefaultPort { get; }
        Int32 DefaultBlockSize { get; }
        Byte[] Encode(Message msg);
        Message Decode(Byte[] bytes);
        OptionType GetOptionType(Int32 optionNumber);
    }
#endif

    #region CoAP 03
#if COAPALL || COAP03
#if COAP03
    public static class Spec
#else
    class Draft03 : ISpec
#endif
    {
        const Int32 VersionBits = 2;
        const Int32 TypeBits = 2;
        const Int32 OptionCountBits = 4;
        const Int32 CodeBits = 8;
        const Int32 IDBits = 16;
        const Int32 OptionDeltaBits = 4;
        const Int32 OptionLengthBaseBits = 4;
        const Int32 OptionLengthExtendedBits = 8;
        const Int32 MaxOptionDelta = (1 << OptionDeltaBits) - 1;
        const Int32 MaxOptionLengthBase = (1 << OptionLengthBaseBits) - 2;
        const Int32 FencepostDivisor = 14;

        static readonly ILogger log = LogManager.GetLogger(typeof(Spec));

#if COAP03
        public const String Name = "draft-ietf-core-coap-03";
        public const Int32 SupportedVersion = 1;
        public const Int32 DefaultPort = 5683;
        public const Int32 DefaultBlockSize = 512;
#else
        public String Name { get { return "draft-ietf-core-coap-03"; } }
        public Int32 SupportedVersion { get { return 1; } }
        public Int32 DefaultPort { get { return 5683; } }
        public Int32 DefaultBlockSize { get { return 512; } }
#endif

#if COAP03
        public static Byte[] Encode(Message msg)
#else
        public Byte[] Encode(Message msg)
#endif
        {
            // create datagram writer to encode options
            DatagramWriter optWriter = new DatagramWriter();
            Int32 optionCount = 0;
            Int32 lastOptionNumber = 0;

            List<Option> options = (List<Option>)msg.GetOptions();
            Sort.InsertionSort(options, delegate(Option o1, Option o2)
            {
                return GetOptionNumber(o1.Type).CompareTo(GetOptionNumber(o2.Type));
            });

            foreach (Option opt in options)
            {
                if (opt.IsDefault)
                    continue;

                Option opt2 = opt;

                Int32 optNum = GetOptionNumber(opt2.Type);
                Int32 optionDelta = optNum - lastOptionNumber;

                // ensure that option delta value can be encoded correctly
                while (optionDelta > MaxOptionDelta)
                {
                    // option delta is too large to be encoded:
                    // add fencepost options in order to reduce the option delta
                    // get fencepost option that is next to the last option
                    Int32 fencepostNumber = NextFencepost(lastOptionNumber);

                    // calculate fencepost delta
                    Int32 fencepostDelta = fencepostNumber - lastOptionNumber;
                    if (fencepostDelta <= 0)
                    {
                        if (log.IsWarnEnabled)
                            log.Warn("Fencepost liveness violated: delta = " + fencepostDelta);
                    }
                    if (fencepostDelta > MaxOptionDelta)
                    {
                        if (log.IsWarnEnabled)
                            log.Warn("Fencepost safety violated: delta = " + fencepostDelta);
                    }

                    // write fencepost option delta
                    optWriter.Write(fencepostDelta, OptionDeltaBits);
                    // fencepost have an empty value
                    optWriter.Write(0, OptionLengthBaseBits);

                    ++optionCount;
                    lastOptionNumber = fencepostNumber;
                    optionDelta -= fencepostDelta;
                }

                // write option delta
                optWriter.Write(optionDelta, OptionDeltaBits);

                if (opt2.Type == OptionType.ContentType)
                {
                    Int32 ct = opt2.IntValue;
                    Int32 ct2 = MapOutMediaType(ct);
                    if (ct != ct2)
                        opt2 = Option.Create(opt2.Type, ct2);
                }

                // write option length
                Int32 length = opt2.Length;
                if (length <= MaxOptionLengthBase)
                {
                    // use option length base field only to encode
                    // option lengths less or equal than MAX_OPTIONLENGTH_BASE
                    optWriter.Write(length, OptionLengthBaseBits);
                }
                else
                {
                    // use both option length base and extended field
                    // to encode option lengths greater than MAX_OPTIONLENGTH_BASE
                    Int32 baseLength = MaxOptionLengthBase + 1;
                    optWriter.Write(baseLength, OptionLengthBaseBits);

                    Int32 extLength = length - baseLength;
                    optWriter.Write(extLength, OptionLengthExtendedBits);
                }

                // write option value
                optWriter.WriteBytes(opt2.RawValue);

                ++optionCount;
                lastOptionNumber = optNum;
            }

            // create datagram writer to encode message data
            DatagramWriter writer = new DatagramWriter();

            // write fixed-size CoAP headers
            writer.Write(msg.Version, VersionBits);
            writer.Write((Int32)msg.Type, TypeBits);
            writer.Write(optionCount, OptionCountBits);
            writer.Write(MapOutCode(msg.Code), CodeBits);
            writer.Write(msg.ID, IDBits);

            // write options
            writer.WriteBytes(optWriter.ToByteArray());

            //write payload
            writer.WriteBytes(msg.Payload);

            return writer.ToByteArray();
        }

#if COAP03
        public static Message Decode(Byte[] bytes)
#else
        public Message Decode(Byte[] bytes)
#endif
        {
            DatagramReader datagram = new DatagramReader(bytes);

            // read headers
            Int32 version = datagram.Read(VersionBits);
            if (version != SupportedVersion)
                return null;

            MessageType type = (MessageType)datagram.Read(TypeBits);
            Int32 optionCount = datagram.Read(OptionCountBits);
            Int32 code = datagram.Read(CodeBits);

            // create new message with subtype according to code number
            Message msg = Message.Create(MapInCode(code));

            msg.Type = type;
            msg.ID = datagram.Read(IDBits);

            // read options
            Int32 currentOption = 0;
            for (Int32 i = 0; i < optionCount; i++)
            {
                // read option delta bits
                Int32 optionDelta = datagram.Read(OptionDeltaBits);

                currentOption += optionDelta;
                OptionType currentOptionType = GetOptionType(currentOption);

                if (IsFencepost(currentOptionType))
                {
                    // read number of options
                    datagram.Read(OptionLengthBaseBits);
                }
                else
                {
                    // read option length
                    Int32 length = datagram.Read(OptionLengthBaseBits);
                    if (length > MaxOptionLengthBase)
                    {
                        // read extended option length
                        length += datagram.Read(OptionLengthExtendedBits);
                    }
                    // read option
                    Option opt = Option.Create(currentOptionType);
                    opt.RawValue = datagram.ReadBytes(length);

                    if (opt.Type == OptionType.ContentType)
                    {
                        Int32 ct = opt.IntValue;
                        Int32 ct2 = MapInMediaType(ct);
                        if (ct != ct2)
                            opt = Option.Create(currentOptionType, ct2);
                    }

                    msg.AddOption(opt);
                }
            }

            msg.Payload = datagram.ReadBytesLeft();

            // incoming message already have a token, 
            // including implicit empty token
            msg.RequiresToken = false;

            return msg;
        }

#if COAP03
        public static Int32 GetOptionNumber(OptionType optionType)
#else
        public Int32 GetOptionNumber(OptionType optionType)
#endif
        {
            switch (optionType)
            {
                case OptionType.Reserved:
                    return 0;
                case OptionType.ContentType:
                    return 1;
                case OptionType.MaxAge:
                    return 2;
                case OptionType.ProxyUri:
                    return 3;
                case OptionType.ETag:
                    return 4;
                case OptionType.UriHost:
                    return 5;
                case OptionType.LocationPath:
                    return 6;
                case OptionType.UriPort:
                    return 7;
                case OptionType.LocationQuery:
                    return 8;
                case OptionType.UriPath:
                    return 9;
                case OptionType.Token:
                    return 11;
                case OptionType.UriQuery:
                    return 15;
                case OptionType.Observe:
                    return 10;
                case OptionType.FencepostDivisor:
                    return 14;
                case OptionType.Block2:
                    return 13;
                default:
                    return (Int32)optionType;
            }
        }

#if COAP03
        public static OptionType GetOptionType(Int32 optionNumber)
#else
        public OptionType GetOptionType(Int32 optionNumber)
#endif
        {
            switch (optionNumber)
            {
                case 0:
                    return OptionType.Reserved;
                case 1:
                    return OptionType.ContentType;
                case 2:
                    return OptionType.MaxAge;
                case 3:
                    return OptionType.ProxyUri;
                case 4:
                    return OptionType.ETag;
                case 5:
                    return OptionType.UriHost;
                case 6:
                    return OptionType.LocationPath;
                case 7:
                    return OptionType.UriPort;
                case 8:
                    return OptionType.LocationQuery;
                case 9:
                    return OptionType.UriPath;
                case 11:
                    return OptionType.Token;
                case 15:
                    return OptionType.UriQuery;
                case 10:
                    return OptionType.Observe;
                case 13:
                    return OptionType.Block2;
                case 14:
                    return OptionType.FencepostDivisor;
                default:
                    return (OptionType)optionNumber;
            }
        }

        private static Int32 MapOutCode(Int32 code)
        {
            switch (code)
            {
                case Code.Content:
                    return 80;
                default:
                    return (code >> 5) * 40 + (code & 0xf);
            }
        }

        private static Int32 MapInCode(Int32 code)
        {
            if (code == 80)
                return Code.Content;
            else
                return ((code / 40) << 5) + (code % 40);
        }

        private static Int32 MapOutMediaType(Int32 mediaType)
        {
            switch (mediaType)
            {
                case MediaType.ApplicationXObixBinary:
                    return 48;
                case MediaType.ApplicationFastinfoset:
                    return 49;
                case MediaType.ApplicationSoapFastinfoset:
                    return 50;
                case MediaType.ApplicationJson:
                    return 51;
                default:
                    return mediaType;
            }
        }

        private static Int32 MapInMediaType(Int32 mediaType)
        {
            switch (mediaType)
            {
                case 48:
                    return MediaType.ApplicationXObixBinary;
                case 49:
                    return MediaType.ApplicationFastinfoset;
                case 50:
                    return MediaType.ApplicationSoapFastinfoset;
                case 51:
                    return MediaType.ApplicationJson;
                default:
                    return mediaType;
            }
        }

        private static Boolean IsFencepost(OptionType type)
        {
            return (Int32)type % (Int32)FencepostDivisor == 0;
        }

        private static Int32 NextFencepost(Int32 optionNumber)
        {
            return (optionNumber / (Int32)FencepostDivisor + 1) * (Int32)FencepostDivisor;
        }
    }
#endif
    #endregion

    #region CoAP 08
#if COAPALL || COAP08
#if COAP08
    public static class Spec
#else
    class Draft08 : ISpec
#endif
    {
        const Int32 VersionBits = 2;
        const Int32 TypeBits = 2;
        const Int32 OptionCountBits = 4;
        const Int32 CodeBits = 8;
        const Int32 IDBits = 16;
        const Int32 OptionDeltaBits = 4;
        const Int32 OptionLengthBaseBits = 4;
        const Int32 OptionLengthExtendedBits = 8;
        const Int32 MaxOptionDelta = (1 << OptionDeltaBits) - 1;
        const Int32 MaxOptionLengthBase = (1 << OptionLengthBaseBits) - 2;
        const Int32 FencepostDivisor = 14;

        static readonly ILogger log = LogManager.GetLogger(typeof(Spec));

#if COAP08
        public const String Name = "draft-ietf-core-coap-08";
        public const Int32 SupportedVersion = 1;
        public const Int32 DefaultPort = 5683;
        public const Int32 DefaultBlockSize = 512;
#else
        public String Name { get { return "draft-ietf-core-coap-08"; } }
        public Int32 SupportedVersion { get { return 1; } }
        public Int32 DefaultPort { get { return 5683; } }
        public Int32 DefaultBlockSize { get { return 512; } }
#endif

#if COAP08
        public static Byte[] Encode(Message msg)
#else
        public Byte[] Encode(Message msg)
#endif
        {
            // create datagram writer to encode options
            DatagramWriter optWriter = new DatagramWriter();
            Int32 optionCount = 0;
            Int32 lastOptionNumber = 0;

            List<Option> options = (List<Option>)msg.GetOptions();
            Sort.InsertionSort(options, delegate(Option o1, Option o2)
            {
                return GetOptionNumber(o1.Type).CompareTo(GetOptionNumber(o2.Type));
            });

            foreach (Option opt in options)
            {
                if (opt.IsDefault)
                    continue;

                Int32 optNum = GetOptionNumber(opt.Type);
                Int32 optionDelta = optNum - lastOptionNumber;

                // ensure that option delta value can be encoded correctly
                while (optionDelta > MaxOptionDelta)
                {
                    // option delta is too large to be encoded:
                    // add fencepost options in order to reduce the option delta
                    // get fencepost option that is next to the last option
                    Int32 fencepostNumber = NextFencepost(lastOptionNumber);

                    // calculate fencepost delta
                    Int32 fencepostDelta = fencepostNumber - lastOptionNumber;
                    if (fencepostDelta <= 0)
                    {
                        if (log.IsWarnEnabled)
                            log.Warn("Fencepost liveness violated: delta = " + fencepostDelta);
                    }
                    if (fencepostDelta > MaxOptionDelta)
                    {
                        if (log.IsWarnEnabled)
                            log.Warn("Fencepost safety violated: delta = " + fencepostDelta);
                    }

                    // write fencepost option delta
                    optWriter.Write(fencepostDelta, OptionDeltaBits);
                    // fencepost have an empty value
                    optWriter.Write(0, OptionLengthBaseBits);

                    ++optionCount;
                    lastOptionNumber = fencepostNumber;
                    optionDelta -= fencepostDelta;
                }

                // write option delta
                optWriter.Write(optionDelta, OptionDeltaBits);

                // write option length
                Int32 length = opt.Length;
                if (length <= MaxOptionLengthBase)
                {
                    // use option length base field only to encode
                    // option lengths less or equal than MAX_OPTIONLENGTH_BASE
                    optWriter.Write(length, OptionLengthBaseBits);
                }
                else
                {
                    // use both option length base and extended field
                    // to encode option lengths greater than MAX_OPTIONLENGTH_BASE
                    Int32 baseLength = MaxOptionLengthBase + 1;
                    optWriter.Write(baseLength, OptionLengthBaseBits);

                    Int32 extLength = length - baseLength;
                    optWriter.Write(extLength, OptionLengthExtendedBits);
                }

                // write option value
                optWriter.WriteBytes(opt.RawValue);

                ++optionCount;
                lastOptionNumber = optNum;
            }

            // create datagram writer to encode message data
            DatagramWriter writer = new DatagramWriter();

            // write fixed-size CoAP headers
            writer.Write(msg.Version, VersionBits);
            writer.Write((Int32)msg.Type, TypeBits);
            writer.Write(optionCount, OptionCountBits);
            writer.Write(msg.Code, CodeBits);
            writer.Write(msg.ID, IDBits);

            // write options
            writer.WriteBytes(optWriter.ToByteArray());

            //write payload
            writer.WriteBytes(msg.Payload);

            return writer.ToByteArray();
        }

#if COAP08
        public static Message Decode(Byte[] bytes)
#else
        public Message Decode(Byte[] bytes)
#endif
        {
            DatagramReader datagram = new DatagramReader(bytes);

            // read headers
            Int32 version = datagram.Read(VersionBits);
            if (version != SupportedVersion)
                return null;

            MessageType type = (MessageType)datagram.Read(TypeBits);
            Int32 optionCount = datagram.Read(OptionCountBits);
            Int32 code = datagram.Read(CodeBits);

            // create new message with subtype according to code number
            Message msg = Message.Create(code);

            msg.Type = type;
            msg.ID = datagram.Read(IDBits);

            // read options
            Int32 currentOption = 0;
            for (Int32 i = 0; i < optionCount; i++)
            {
                // read option delta bits
                Int32 optionDelta = datagram.Read(OptionDeltaBits);

                currentOption += optionDelta;
                OptionType currentOptionType = GetOptionType(currentOption);

                if (IsFencepost(currentOptionType))
                {
                    // read number of options
                    datagram.Read(OptionLengthBaseBits);
                }
                else
                {
                    // read option length
                    Int32 length = datagram.Read(OptionLengthBaseBits);
                    if (length > MaxOptionLengthBase)
                    {
                        // read extended option length
                        length += datagram.Read(OptionLengthExtendedBits);
                    }
                    // read option
                    Option opt = Option.Create(currentOptionType);
                    opt.RawValue = datagram.ReadBytes(length);

                    msg.AddOption(opt);
                }
            }

            msg.Payload = datagram.ReadBytesLeft();

            // incoming message already have a token, 
            // including implicit empty token
            msg.RequiresToken = false;

            return msg;
        }

#if COAP08
        public static Int32 GetOptionNumber(OptionType optionType)
#else
        public Int32 GetOptionNumber(OptionType optionType)
#endif
        {
            switch (optionType)
            {
                case OptionType.Reserved:
                    return 0;
                case OptionType.ContentType:
                    return 1;
                case OptionType.MaxAge:
                    return 2;
                case OptionType.ProxyUri:
                    return 3;
                case OptionType.ETag:
                    return 4;
                case OptionType.UriHost:
                    return 5;
                case OptionType.LocationPath:
                    return 6;
                case OptionType.UriPort:
                    return 7;
                case OptionType.LocationQuery:
                    return 8;
                case OptionType.UriPath:
                    return 9;
                case OptionType.Token:
                    return 11;
                case OptionType.UriQuery:
                    return 15;
                case OptionType.Observe:
                    return 10;
                case OptionType.Accept:
                    return 12;
                case OptionType.IfMatch:
                    return 13;
                case OptionType.FencepostDivisor:
                    return 14;
                case OptionType.Block2:
                    return 17;
                case OptionType.Block1:
                    return 19;
                case OptionType.IfNoneMatch:
                    return 21;
                default:
                    return (Int32)optionType;
            }
        }

#if COAP08
        public static OptionType GetOptionType(Int32 optionNumber)
#else
        public OptionType GetOptionType(Int32 optionNumber)
#endif
        {
            switch (optionNumber)
            {
                case 0:
                    return OptionType.Reserved;
                case 1:
                    return OptionType.ContentType;
                case 2:
                    return OptionType.MaxAge;
                case 3:
                    return OptionType.ProxyUri;
                case 4:
                    return OptionType.ETag;
                case 5:
                    return OptionType.UriHost;
                case 6:
                    return OptionType.LocationPath;
                case 7:
                    return OptionType.UriPort;
                case 8:
                    return OptionType.LocationQuery;
                case 9:
                    return OptionType.UriPath;
                case 11:
                    return OptionType.Token;
                case 15:
                    return OptionType.UriQuery;
                case 10:
                    return OptionType.Observe;
                case 12:
                    return OptionType.Accept;
                case 13:
                    return OptionType.IfMatch;
                case 14:
                    return OptionType.FencepostDivisor;
                case 17:
                    return OptionType.Block2;
                case 19:
                    return OptionType.Block1;
                case 21:
                    return OptionType.IfNoneMatch;
                default:
                    return (OptionType)optionNumber;
            }
        }

        /// <summary>
        /// Checks whether an option is a fencepost option.
        /// </summary>
        /// <param name="type">The option type to check</param>
        /// <returns>True iff the option is a fencepost option</returns>
        public static Boolean IsFencepost(OptionType type)
        {
            return (Int32)type % (Int32)FencepostDivisor == 0;
        }

        /// <summary>
        /// Returns the next fencepost option number following a given option number.
        /// </summary>
        /// <param name="optionNumber">The option number</param>
        /// <returns>The smallest fencepost option number larger than the given option</returns>
        public static Int32 NextFencepost(Int32 optionNumber)
        {
            return (optionNumber / (Int32)FencepostDivisor + 1) * (Int32)FencepostDivisor;
        }
    }
#endif
    #endregion

    #region CoAP 12
#if COAPALL || COAP12
#if COAP12
    public static class Spec
#else
    class Draft12 : ISpec
#endif
    {
        const Int32 VersionBits = 2;
        const Int32 TypeBits = 2;
        const Int32 OptionCountBits = 4;
        const Int32 CodeBits = 8;
        const Int32 IDBits = 16;
        const Int32 OptionDeltaBits = 4;
        const Int32 OptionLengthBaseBits = 4;
        const Int32 OptionLengthExtendedBits = 8;
        const Int32 MaxOptionDelta = 14;
        const Int32 SingleOptionJumpBits = 8;
        const Int32 MaxOptionLengthBase = (1 << OptionLengthBaseBits) - 2;

#if COAP12
        public const String Name = "draft-ietf-core-coap-12";
        public const Int32 SupportedVersion = 1;
        public const Int32 DefaultPort = 5683;
        public const Int32 DefaultBlockSize = 512;
#else
        public String Name { get { return "draft-ietf-core-coap-12"; } }
        public Int32 SupportedVersion { get { return 1; } }
        public Int32 DefaultPort { get { return 5683; } }
        public Int32 DefaultBlockSize { get { return 512; } }
#endif

#if COAP12
        public static Byte[] Encode(Message msg)
#else
        public Byte[] Encode(Message msg)
#endif
        {
            // create datagram writer to encode options
            DatagramWriter optWriter = new DatagramWriter();
            Int32 optionCount = 0;
            Int32 lastOptionNumber = 0;

            List<Option> options = (List<Option>)msg.GetOptions();
            Sort.InsertionSort(options, delegate(Option o1, Option o2)
            {
                return GetOptionNumber(o1.Type).CompareTo(GetOptionNumber(o2.Type));
            });

            foreach (Option opt in options)
            {
                if (opt.IsDefault)
                    continue;

                Int32 optNum = GetOptionNumber(opt.Type);
                Int32 optionDelta = optNum - lastOptionNumber;

                /*
                 * The Option Jump mechanism is used when the delta to the next option
                 * number is larger than 14.
                 */
                if (optionDelta > MaxOptionDelta)
                {
                    /*
                     * For the formats that include an Option Jump Value, the actual
                     * addition to the current Option number is computed as follows:
                     * Delta = ((Option Jump Value) + N) * 8 where N is 2 for the
                     * one-byte version and N is 258 for the two-byte version.
                     */
                    if (optionDelta < 30)
                    {
                        optWriter.Write(0xF1, SingleOptionJumpBits);
                        optionDelta -= 15;
                    }
                    else if (optionDelta < 2064)
                    {
                        Int32 optionJumpValue = (optionDelta / 8) - 2;
                        optionDelta -= (optionJumpValue + 2) * 8;
                        optWriter.Write(0xF2, SingleOptionJumpBits);
                        optWriter.Write(optionJumpValue, SingleOptionJumpBits);
                    }
                    else if (optionDelta < 526359)
                    {
                        optionDelta = Math.Min(optionDelta, 526344); // Limit to avoid overflow
                        Int32 optionJumpValue = (optionDelta / 8) - 258;
                        optionDelta -= (optionJumpValue + 258) * 8;
                        optWriter.Write(0xF3, SingleOptionJumpBits);
                        optWriter.Write(optionJumpValue, 2 * SingleOptionJumpBits);
                    }
                    else
                    {
                        throw new Exception("Option delta too large. Actual delta: " + optionDelta);
                    }
                }

                // write option delta
                optWriter.Write(optionDelta, OptionDeltaBits);

                // write option length
                Int32 length = opt.Length;
                if (length <= MaxOptionLengthBase)
                {
                    // use option length base field only to encode
                    // option lengths less or equal than MAX_OPTIONLENGTH_BASE
                    optWriter.Write(length, OptionLengthBaseBits);
                }
                else if (length <= 1034)
                {
                    /*
                     * When the Length field is set to 15, another byte is added as
                     * an 8-bit unsigned integer whose value is added to the 15,
                     * allowing option value lengths of 15-270 bytes. For option
                     * lengths beyond 270 bytes, we reserve the value 255 of an
                     * extension byte to mean
                     * "add 255, read another extension byte". Options that are
                     * longer than 1034 bytes MUST NOT be sent
                     */
                    optWriter.Write(15, OptionLengthBaseBits);

                    Int32 rounds = (length - 15) / 255;
                    for (Int32 i = 0; i < rounds; i++)
                    {
                        optWriter.Write(255, OptionLengthExtendedBits);
                    }
                    Int32 remainingLength = length - ((rounds * 255) + 15);
                    optWriter.Write(remainingLength, OptionLengthExtendedBits);
                }
                else
                {
                    throw new Exception("Option length larger than allowed 1034. Actual length: " + length);
                }

                // write option value
                if (length > 0)
                    optWriter.WriteBytes(opt.RawValue);

                ++optionCount;
                lastOptionNumber = optNum;
            }

            // create datagram writer to encode message data
            DatagramWriter writer = new DatagramWriter();

            // write fixed-size CoAP headers
            writer.Write(msg.Version, VersionBits);
            writer.Write((Int32)msg.Type, TypeBits);
            if (optionCount < 15)
                writer.Write(optionCount, OptionCountBits);
            else
                writer.Write(15, OptionCountBits);
            writer.Write(msg.Code, CodeBits);
            writer.Write(msg.ID, IDBits);

            // write options
            writer.WriteBytes(optWriter.ToByteArray());

            if (optionCount > 14)
            {
                // end-of-options marker when there are more than 14 options
                writer.Write(0xf0, 8);
            }

            //write payload
            writer.WriteBytes(msg.Payload);

            return writer.ToByteArray();
        }

#if COAP12
        public static Message Decode(Byte[] bytes)
#else
        public Message Decode(Byte[] bytes)
#endif
        {
            DatagramReader datagram = new DatagramReader(bytes);

            // read headers
            Int32 version = datagram.Read(VersionBits);
            if (version != SupportedVersion)
                return null;

            MessageType type = (MessageType)datagram.Read(TypeBits);
            Int32 optionCount = datagram.Read(OptionCountBits);
            Int32 code = datagram.Read(CodeBits);

            // create new message with subtype according to code number
            Message msg = Message.Create(code);

            msg.Type = type;
            msg.ID = datagram.Read(IDBits);

            // read options
            Int32 currentOption = 0;
            Boolean hasMoreOptions = optionCount == 15;
            for (Int32 i = 0; (i < optionCount || hasMoreOptions) && datagram.BytesAvailable; i++)
            {
                // first 4 option bits: either option jump or option delta
                Int32 optionDelta = datagram.Read(OptionDeltaBits);

                if (optionDelta == 15)
                {
                    // option jump or end-of-options marker
                    Int32 bits = datagram.Read(4);
                    switch (bits)
                    {
                        case 0:
                            // end-of-options marker read (0xF0), payload follows
                            hasMoreOptions = false;
                            continue;
                        case 1:
                            // 0xF1 (Delta = 15)
                            optionDelta = 15 + datagram.Read(OptionDeltaBits);
                            break;
                        case 2:
                            // Delta = ((Option Jump Value) + 2) * 8
                            optionDelta = (datagram.Read(8) + 2) * 8 + datagram.Read(OptionDeltaBits);
                            break;
                        case 3:
                            // Delta = ((Option Jump Value) + 258) * 8
                            optionDelta = (datagram.Read(16) + 258) * 8 + datagram.Read(OptionDeltaBits);
                            break;
                        default:
                            break;
                    }
                }

                currentOption += optionDelta;
                OptionType currentOptionType = GetOptionType(currentOption);

                Int32 length = datagram.Read(OptionLengthBaseBits);
                if (length == 15)
                {
                    /*
                     * When the Length field is set to 15, another byte is added as
                     * an 8-bit unsigned integer whose value is added to the 15,
                     * allowing option value lengths of 15-270 bytes. For option
                     * lengths beyond 270 bytes, we reserve the value 255 of an
                     * extension byte to mean
                     * "add 255, read another extension byte".
                     */
                    Int32 additionalLength = 0;
                    do
                    {
                        additionalLength = datagram.Read(8);
                        length += additionalLength;
                    } while (additionalLength >= 255);
                }

                // read option
                Option opt = Option.Create(currentOptionType);
                opt.RawValue = datagram.ReadBytes(length);

                msg.AddOption(opt);
            }

            msg.Payload = datagram.ReadBytesLeft();

            // incoming message already have a token, including implicit empty token
            msg.RequiresToken = false;

            return msg;
        }

#if COAP12
        public static Int32 GetOptionNumber(OptionType optionType)
#else
        public Int32 GetOptionNumber(OptionType optionType)
#endif
        {
            return (Int32)optionType;
        }

#if COAP12
        public static OptionType GetOptionType(Int32 optionNumber)
#else
        public OptionType GetOptionType(Int32 optionNumber)
#endif
        {
            return (OptionType)optionNumber;
        }
    }
#endif
    #endregion

    #region CoAP 13
#if COAPALL || COAP13
#if COAP13
    public static class Spec
#else
    class Draft13 : ISpec
#endif
    {
        const Int32 VersionBits = 2;
        const Int32 TypeBits = 2;
        const Int32 TokenLengthBits = 4;
        const Int32 CodeBits = 8;
        const Int32 IDBits = 16;
        const Int32 OptionDeltaBits = 4;
        const Int32 OptionLengthBits = 4;
        const Byte PayloadMarker = 0xFF;

        static readonly ILogger log = LogManager.GetLogger(typeof(Spec));

#if COAP13
        public const String Name = "draft-ietf-core-coap-13";
        public const Int32 SupportedVersion = 1;
        public const Int32 DefaultPort = 5683;
        public const Int32 DefaultBlockSize = 512;
#else
        public String Name { get { return "draft-ietf-core-coap-13"; } }
        public Int32 SupportedVersion { get { return 1; } }
        public Int32 DefaultPort { get { return 5683; } }
        public Int32 DefaultBlockSize { get { return 512; } }
#endif

#if COAP13
        public static Byte[] Encode(Message msg)
#else
        public Byte[] Encode(Message msg)
#endif
        {
            // create datagram writer to encode message data
            DatagramWriter writer = new DatagramWriter();

            // write fixed-size CoAP headers
            writer.Write(msg.Version, VersionBits);
            writer.Write((Int32)msg.Type, TypeBits);
            writer.Write(msg.Token.Length, TokenLengthBits);
            writer.Write(msg.Code, CodeBits);
            writer.Write(msg.ID, IDBits);

            // write token, which may be 0 to 8 bytes, given by token length field
            writer.WriteBytes(msg.Token);

            Int32 lastOptionNumber = 0;
            List<Option> options = (List<Option>)msg.GetOptions();
            Sort.InsertionSort(options, delegate(Option o1, Option o2)
            {
                return GetOptionNumber(o1.Type).CompareTo(GetOptionNumber(o2.Type));
            });

            foreach (Option opt in options)
            {
                if (opt.Type == OptionType.Token)
                    continue;
                if (opt.IsDefault)
                    continue;

                // write 4-bit option delta
                Int32 optNum = GetOptionNumber(opt.Type);
                Int32 optionDelta = optNum - lastOptionNumber;
                Int32 optionDeltaNibble = GetOptionNibble(optionDelta);
                writer.Write(optionDeltaNibble, OptionDeltaBits);

                // write 4-bit option length
                Int32 optionLength = opt.Length;
                Int32 optionLengthNibble = GetOptionNibble(optionLength);
                writer.Write(optionLengthNibble, OptionLengthBits);

                // write extended option delta field (0 - 2 bytes)
                if (optionDeltaNibble == 13)
                {
                    writer.Write(optionDelta - 13, 8);
                }
                else if (optionDeltaNibble == 14)
                {
                    writer.Write(optionDelta - 269, 16);
                }

                // write extended option length field (0 - 2 bytes)
                if (optionLengthNibble == 13)
                {
                    writer.Write(optionLength - 13, 8);
                }
                else if (optionLengthNibble == 14)
                {
                    writer.Write(optionLength - 269, 16);
                }

                // write option value
                writer.WriteBytes(opt.RawValue);

                lastOptionNumber = optNum;
            }

            if (msg.Payload != null && msg.Payload.Length > 0)
            {
                // if payload is present and of non-zero length, it is prefixed by
                // an one-byte Payload Marker (0xFF) which indicates the end of
                // options and the start of the payload
                writer.WriteByte(PayloadMarker);
            }
            //write payload
            writer.WriteBytes(msg.Payload);

            return writer.ToByteArray();
        }

#if COAP13
        public static Message Decode(Byte[] bytes)
#else
        public Message Decode(Byte[] bytes)
#endif
        {
            DatagramReader datagram = new DatagramReader(bytes);

            // read headers
            Int32 version = datagram.Read(VersionBits);
            if (version != SupportedVersion)
                return null;

            MessageType type = (MessageType)datagram.Read(TypeBits);
            Int32 tokenLength = datagram.Read(TokenLengthBits);
            Int32 code = datagram.Read(CodeBits);

            // create new message with subtype according to code number
            Message msg = Message.Create(code);

            msg.Type = type;
            msg.ID = datagram.Read(IDBits);

            // read token
            if (tokenLength > 0)
                msg.Token = datagram.ReadBytes(tokenLength);
            else
                msg.RequiresToken = false;

            // read options
            Int32 currentOption = 0;
            while (datagram.BytesAvailable)
            {
                Byte nextByte = datagram.ReadNextByte();
                if (nextByte == PayloadMarker)
                {
                    if (!datagram.BytesAvailable)
                        // the presence of a marker followed by a zero-length payload
                        // must be processed as a message format error
                        return null;

                    msg.Payload = datagram.ReadBytesLeft();
                }
                else
                {
                    // the first 4 bits of the byte represent the option delta
                    Int32 optionDeltaNibble = (0xF0 & nextByte) >> 4;
                    currentOption += GetValueFromOptionNibble(optionDeltaNibble, datagram);

                    // the second 4 bits represent the option length
                    Int32 optionLengthNibble = (0x0F & nextByte);
                    Int32 optionLength = GetValueFromOptionNibble(optionLengthNibble, datagram);

                    // read option
                    OptionType currentOptionType = GetOptionType(currentOption);
                    Option opt = Option.Create(currentOptionType);
                    opt.RawValue = datagram.ReadBytes(optionLength);

                    msg.AddOption(opt);
                }
            }

            return msg;
        }

#if COAP13
        public static Int32 GetOptionNumber(OptionType optionType)
#else
        public Int32 GetOptionNumber(OptionType optionType)
#endif
        {
            return (Int32)optionType;
        }

#if COAP13
        public static OptionType GetOptionType(Int32 optionNumber)
#else
        public OptionType GetOptionType(Int32 optionNumber)
#endif
        {
            return (OptionType)optionNumber;
        }

        /// <summary>
        /// Calculates the value used in the extended option fields as specified
        /// in draft-ietf-core-coap-13, section 3.1.
        /// </summary>
        /// <param name="nibble">the 4-bit option header value</param>
        /// <param name="datagram">the datagram</param>
        /// <returns>the value calculated from the nibble and the extended option value</returns>
        private static Int32 GetValueFromOptionNibble(Int32 nibble, DatagramReader datagram)
        {
            if (nibble < 13)
            {
                return nibble;
            }
            else if (nibble == 13)
            {
                return datagram.Read(8) + 13;
            }
            else if (nibble == 14)
            {
                return datagram.Read(16) + 269;
            }
            else
            {
                // TODO error
                if (log.IsWarnEnabled)
                    log.Warn("15 is reserved for payload marker, message format error");
                return 0;
            }
        }

        /// <summary>
        /// Returns the 4-bit option header value.
        /// </summary>
        /// <param name="optionValue">the option value (delta or length) to be encoded</param>
        /// <returns>the 4-bit option header value</returns>
        private static Int32 GetOptionNibble(Int32 optionValue)
        {
            if (optionValue <= 12)
            {
                return optionValue;
            }
            else if (optionValue <= 255 + 13)
            {
                return 13;
            }
            else if (optionValue <= 65535 + 269)
            {
                return 14;
            }
            else
            {
                // TODO format error
                if (log.IsWarnEnabled)
                    log.Warn("The option value (" + optionValue + ") is too large to be encoded; Max allowed is 65804.");
                return 0;
            }
        }
    }
#endif
    #endregion
}
