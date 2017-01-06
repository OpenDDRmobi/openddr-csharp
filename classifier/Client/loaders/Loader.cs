#region "Header"
/*
 * Copyright (c) 2011-2017 OpenDDR LLC and others. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing,
 *  software distributed under the License is distributed on an
 *  "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 *  KIND, either express or implied.  See the License for the
 *  specific language governing permissions and limitations
 *  under the License.
 */
#endregion ' Header
//
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
//
namespace ClassifierClient
{
    /// <summary>
    ///  Load Device and Pattern data from StreamReader
    /// </summary>
    /// <author>eberhard speer jr.</author>
    /// <remarks>-</remarks>
    internal sealed class Loader
    {
        //
        private IDictionary<string, Device> deviceList { get; set; }

        #region "Properties"
        /// <summary>
        ///  Returns Device data dictionary
        /// </summary>
        /// <returns>IDictionary(Of String, Device)</returns>
        /// <remarks>-</remarks>
        public IDictionary<string, Device> Devices
        {
            get { return deviceList; }
        }
        #endregion ' Properties

        #region "Constructor"
        /// <summary>
        ///  Default new Device data Loader
        /// </summary>
        /// <exception cref="OpenDDRException">Thrown when (InnerException)<ul>
        ///                                                   <li>NullReferenceException : OpenDDR ConnectionStrings missing in config file</li>
        ///                                                   <li>WebException : URL Loader exception</li>
        ///                                                   <li>ArgumentException : File Loader exception</li> 
        ///                                                   <li>ArgumentException : Loader exception</li> 
        ///                                                  </ul>
        /// </exception>
        /// <remarks>-</remarks>
        public Loader()
        {
            deviceList = new Dictionary<string, Device>();
            try
            {
                string folder = ConfigurationManager.ConnectionStrings[Constants.APP_NAME].ToString().Trim().ToLowerInvariant();
                // Devices
                string[] devs = { Constants.DEVICE_DATA, Constants.DEVICE_DATA_PATCH };
                string[] patts = { Constants.BUILDER_DATA, Constants.BUILDER_DATA_PATCH };
                if (folder.StartsWith(Constants.HTTP_PREFIX))
                {
                    char[] fs = { '/' };
                    folder = folder.TrimEnd(fs);

                    foreach (string xmlFile in devs)
                    {
                        LoadDeviceData(new UrlLoader(string.Format("{0}/{1}", folder, xmlFile)).Reader);
                    }
                    // Patterns
                    foreach (string xmlFile in patts)
                    {
                        LoadDevicePatterns(new UrlLoader(string.Format("{0}/{1}", folder, xmlFile)).Reader);
                    }
                }
                else
                {
                    char[] bs = { '\\' };
                    folder = folder.TrimEnd(bs);
                    // Devices
                    foreach (string xmlFile in devs)
                    {
                        LoadDeviceData(new FileLoader(string.Format("{0}/{1}", folder, xmlFile)).Reader);
                    }
                    // Patterns
                    foreach (string xmlFile in patts)
                    {
                        LoadDevicePatterns(new FileLoader(string.Format("{0}/{1}", folder, xmlFile)).Reader);
                    }
                }
            }
            catch (System.NullReferenceException ex)
            {
                throw new OpenDDRException(string.Format(Constants.CONFIG_ERROR_CONN_FORMAT, AppDomain.CurrentDomain.SetupInformation.ConfigurationFile), ex);
            }
            catch (System.Net.WebException ex)
            {
                throw new OpenDDRException(string.Format(Constants.WEB_ERROR_FORMAT, ex.Message), ex);
            }
            catch (System.ArgumentException ex)
            {
                throw new OpenDDRException(ex.Message, ex);
            }
            catch (System.Exception ex)
            {
                throw new OpenDDRException(ex.Message, ex);
            }
        }
        #endregion ' Constructor

        #region "Methods"
        /// <summary>
        ///  Load Device data from StreamReader
        /// </summary>
        /// <param name="inSteam">StreamReader</param>
        /// <remarks>-</remarks>
        private void LoadDeviceData(StreamReader inSteam)
        {
            XmlParser parser = new XmlParser(inSteam);
            string tag = string.Empty;
            try
            {
                Device device = new Device();
                Dictionary<string, string> attributes = new Dictionary<string, string>();
                while ((tag = parser.NextTag).Length > 0)
                {
                    if (tag.StartsWith("<device "))
                    {
                        device.Id = XmlParser.getAttribute(tag, "id");
                        device.ParentId = XmlParser.getAttribute(tag, "parentId");
                    }
                    else if (tag.Equals("</device>"))
                    {
                        if (!string.IsNullOrEmpty(device.Id))
                        {
                            attributes["id"] = device.Id;
                            device.Attributes = attributes;
                            if (Devices.ContainsKey(device.Id))
                            {
                                Devices[device.Id] = device;
                            }
                            else
                            {
                                Devices.Add(device.Id, device);
                            }
                        }
                        // reset
                        device = new Device();
                        attributes = new Dictionary<string, string>();
                    }
                    else if (tag.StartsWith("<property "))
                    {
                        string key = XmlParser.getAttribute(tag, "name");
                        string value = XmlParser.getAttribute(tag, "value");
                        attributes[key] = value;
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw new System.ArgumentException(string.Format("loadDeviceData : {0}", ex.Message), ex);
            }
        }
        /// <summary>
        ///  Load Device Pattern data from StreamReader
        /// </summary>
        /// <param name="inStream">StreamReader</param>
        /// <remarks></remarks>
        private void LoadDevicePatterns(StreamReader inStream)
        {
            XmlParser parser = new XmlParser(inStream);
            string tag = "";
            try
            {
                string builder = "";
                Device device = null;
                string id = "";
                List<string> patterns = new List<string>();
                while ((tag = parser.NextTag).Length > 0)
                {
                    if (tag.StartsWith("<builder "))
                    {
                        builder = XmlParser.getAttribute(tag, "class");
                        if (builder.LastIndexOf(".") >= 0)
                        {
                            builder = builder.Substring(builder.LastIndexOf(".") + 1);
                        }
                    }
                    else if (tag.StartsWith("<device "))
                    {
                        device = Devices[XmlParser.getAttribute(tag, "id")];
                    }
                    else if (tag.Equals("</device>"))
                    {
                        if (device != null)
                        {
                            if (builder.Equals("TwoStepDeviceBuilder"))
                            {
                                device.Patterns.AndPattern = patterns;
                                string unigram = "";
                                foreach (string pattern in patterns)
                                {
                                    if (pattern.Contains(unigram))
                                    {
                                        unigram = pattern;
                                    }
                                    else
                                    {
                                        unigram += pattern;
                                    }
                                }
                                device.Patterns.AddPattern = unigram;
                            }
                            else
                            {
                                device.Patterns.OrPattern = patterns;
                            }
                            if (builder.Equals("SimpleDeviceBuilder"))
                            {
                                device.Type = "simple";
                            }
                            else
                            {
                                device.Type = "weak";
                            }
                        }
                        else
                        {
                            Util.Log("ERROR: device not found: '" + id + "'");
                        }
                        // reset
                        device = null;
                        id = "";
                        patterns = new List<string>();
                    }
                    else if (tag.Equals("<value>"))
                    {
                        string pattern = Util.Normalize(parser.TagValue);
                        if (string.IsNullOrEmpty(pattern))
                        {
                            continue;
                        }
                        patterns.Add(pattern);
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw new System.ArgumentException(string.Format("loadDevicePatterns : {0}", ex.Message), ex);
            }
        }
        /// <summary>
        ///  Recursively add Device's Parent Attributes
        /// </summary>
        /// <param name="device">Device</param>
        /// <remarks>-</remarks>
        private void MergeParent(Device device)
        {
            string parentId = device.ParentId;
            if (string.IsNullOrEmpty(parentId))
            {
                return;
            }
            Device parent = null;
            if (!deviceList.TryGetValue(parentId, out parent))
            {
                return;
            }
            MergeParent(parent);
            foreach (string key in parent.Attributes.Keys)
            {
                if (!device.Attributes.ContainsKey(key))
                {
                    device.Attributes[key] = parent.Attributes[key];
                }
            }
        }
        /// <summary>
        ///  Sets Parent device attributes
        /// </summary>
        /// <remarks>-</remarks>
        private void setParentAttributes()
        {
            foreach (Device device in deviceList.Values)
            {
                MergeParent(device);
            }
        }
        #endregion ' Methods
    }
}