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

namespace CoAP
{
    /// <summary>
    /// This class describes the CoAP Media Type Registry as defined in
    /// draft-ietf-core-coap (07), section 11.3.
    /// </summary>
    public class MediaType
    {
        /// <summary>
        /// undefined
        /// </summary>
        public const Int32 Undefined = -1;
        /// <summary>
        /// text/plain; charset=utf-8
        /// </summary>
        public const Int32 TextPlain = 0;
        /// <summary>
        /// text/xml
        /// </summary>
        public const Int32 TextXml = 1;
        /// <summary>
        /// text/csv
        /// </summary>
        public const Int32 TextCsv = 2;
        /// <summary>
        /// text/html
        /// </summary>
        public const Int32 TextHtml = 3;
        /// <summary>
        /// image/gif
        /// </summary>
        public const Int32 ImageGif = 21;
        /// <summary>
        /// image/jpeg
        /// </summary>
        public const Int32 ImageJpeg = 22;
        /// <summary>
        /// image/png
        /// </summary>
        public const Int32 ImagePng = 23;
        /// <summary>
        /// image/tiff
        /// </summary>
        public const Int32 ImageTiff = 24;
        /// <summary>
        /// audio/raw
        /// </summary>
        public const Int32 AudioRaw = 25;
        /// <summary>
        /// video/raw
        /// </summary>
        public const Int32 VideoRaw = 26;
        /// <summary>
        /// application/link-format
        /// </summary>
        public const Int32 ApplicationLinkFormat = 40;
        /// <summary>
        /// application/xml
        /// </summary>
        public const Int32 ApplicationXml = 41;
        /// <summary>
        /// application/octet-stream
        /// </summary>
        public const Int32 ApplicationOctetStream = 42;
        /// <summary>
        /// application/rdf+xml
        /// </summary>
        public const Int32 ApplicationRdfXml = 43;
        /// <summary>
        /// application/soap+xml
        /// </summary>
        public const Int32 ApplicationSoapXml = 44;
        /// <summary>
        /// application/atom+xml
        /// </summary>
        public const Int32 ApplicationAtomXml = 45;
        /// <summary>
        /// application/xmpp+xml
        /// </summary>
        public const Int32 ApplicationXmppXml = 46;
        /// <summary>
        /// application/exi
        /// </summary>
        public const Int32 ApplicationExi = 47;
        /// <summary>
        /// application/fastinfoset
        /// </summary>
        public const Int32 ApplicationFastinfoset = 48;
        /// <summary>
        /// application/soap+fastinfoset
        /// </summary>
        public const Int32 ApplicationSoapFastinfoset = 49;
        /// <summary>
        /// application/json
        /// </summary>
        public const Int32 ApplicationJson = 50;
        /// <summary>
        /// application/x-obix-binary
        /// </summary>
        public const Int32 ApplicationXObixBinary = 51;
        /// <summary>
        /// any
        /// </summary>
        public const Int32 Any = 0xFF;

        private static readonly Dictionary<Int32, String[]> registry = new Dictionary<Int32, String[]>();

        static MediaType()
        {
            registry.Add(TextPlain, new String[] { "text/plain", "txt" });
            registry.Add(TextXml, new String[] { "text/xml", "xml" });
            registry.Add(TextCsv, new String[] { "text/csv", "csv" });
            registry.Add(TextHtml, new String[] { "text/html", "html" });

            registry.Add(ImageGif, new String[] { "image/gif", "gif" });
            registry.Add(ImageJpeg, new String[] { "image/jpeg", "jpg" });
            registry.Add(ImagePng, new String[] { "image/png", "png" });
            registry.Add(ImageTiff, new String[] { "image/tiff", "tif" });
            registry.Add(AudioRaw, new String[] { "audio/raw", "raw" });
            registry.Add(VideoRaw, new String[] { "video/raw", "raw" });

            registry.Add(ApplicationLinkFormat, new String[] { "application/link-format", "wlnk" });
            registry.Add(ApplicationXml, new String[] { "application/xml", "xml" });
            registry.Add(ApplicationOctetStream, new String[] { "application/octet-stream", "bin" });
            registry.Add(ApplicationRdfXml, new String[] {"application/rdf+xml", "rdf"});
            registry.Add(ApplicationSoapXml, new String[] {"application/soap+xml", "soap"});
            registry.Add(ApplicationAtomXml, new String[] {"application/atom+xml", "atom"});
            registry.Add(ApplicationXmppXml, new String[] {"application/xmpp+xml", "xmpp"});
            registry.Add(ApplicationFastinfoset,new String[] { "application/fastinfoset", "finf"});
            registry.Add(ApplicationSoapFastinfoset, new String[] {"application/soap+fastinfoset", "soap.finf"});
            registry.Add(ApplicationXObixBinary, new String[] { "application/x-obix-binary", "obix" });
            registry.Add(ApplicationExi, new String[] { "application/exi", "exi" });
            registry.Add(ApplicationJson, new String[] { "application/json", "json" });
        }

        /// <summary>
        /// Checks whether the given media type is a type of image.
        /// </summary>
        /// <param name="mediaType">The media type to be checked</param>
        /// <returns>True iff the media type is a type of image</returns>
        public static Boolean IsImage(Int32 mediaType)
        {
            return mediaType >= ImageGif && mediaType <= ImageTiff;
        }

        /// <summary>
        /// Returns a string representation of the media type.
        /// </summary>
        /// <param name="mediaType">The media type to be described</param>
        /// <returns>A string describing the media type</returns>
        public static String ToString(Int32 mediaType)
        {
            if (registry.ContainsKey(mediaType))
            {
                return registry[mediaType][0];
            }
            else
            {
                return "Unknown media type: " + mediaType;
            }
        }

        /// <summary>
        /// Gets the file extension of the given media type.
        /// </summary>
        public static String ToFileExtension(Int32 mediaType)
        {
            if (registry.ContainsKey(mediaType))
            {
                return registry[mediaType][1];
            }
            else
            {
                return "unknown";
            }
        }

        public static Int32 NegotiationContent(Int32 defaultContentType, IEnumerable<Int32> supported, IEnumerable<Option> accepted)
        {
            if (accepted == null)
                return defaultContentType;

            Boolean hasAccept = false;
            foreach (Option accept in accepted)
            {
                foreach (Int32 ct in supported)
                {
                    if (ct == accept.IntValue)
                        return ct;
                }
                hasAccept = true;
            }
            return hasAccept ? Undefined : defaultContentType;
        }
    }
}
