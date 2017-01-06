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
#endregion
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
//
namespace ClassifierClient
{
    /// <summary>
    ///  Classifier User-Agent Resolver
    /// </summary>
    /// <author>Eberhard Speer jr.</author>
    /// <author>Werner Keil</author>
    /// <remarks>OpenDDR Classifier Client C# version 
    ///          inspired by OpenDDR Classifier.java</remarks>
    public sealed class Classifier
    {
        //
        internal IDictionary<string, Device> devices;
        internal IDictionary<string, List<Device>> patterns;

        #region "Properties"
        /// <summary>
        ///  Returns number of Devices in Device data
        /// </summary>
        /// <returns>Integer</returns>
        /// <remarks>-</remarks>
        public int DeviceCount
        {
            get { return devices.Count; }
        }
        /// <summary>
        ///  Returns number of Device Patterns in Device pattern data
        /// </summary>
        /// <returns>Integer</returns>
        /// <remarks>-</remarks>
        public int PatternCount
        {
            get { return patterns.Count; }
        }
        /// <summary>
        ///  Main Release and  build version
        /// </summary>
        /// <returns>String</returns>
        /// <remarks>-</remarks>
        public string Version
        {
            get { return string.Format(Constants.VERSION_FORMAT, Constants.RELEASE_VERSION, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()); }
        }
        #endregion

        #region "Constructor"
        /// <summary>
        /// New Device User-Agent Resolver
        /// </summary>
        /// <remarks>-</remarks>
        public Classifier()
        {
            devices = new Dictionary<string, Device>();
            patterns = new Dictionary<string, List<Device>>();
            devices = new Loader().Devices;
            CreateIndex();
        }
        #endregion

        #region "Methods"
        /// <summary>
        ///  Create Device and Pattern Index 
        /// </summary>
        /// <remarks>-</remarks>
        private void CreateIndex()
        {
            foreach (Device device in devices.Values)
            {
                foreach (IList<string> patternset in device.Patterns.Patterns)
                {
                    for (int i = 0; i <= patternset.Count - 1; i++)
                    {
                        string pattern = patternset[i];
                        // deal with duplicates
                        if (patterns.ContainsKey(pattern))
                        {
                            if (i == (patternset.Count - 1) && !patterns[pattern].Contains(device))
                            {
                                patterns[pattern].Add(device);
                            }
                        }
                        else
                        {
                            List<Device> subList = new List<Device>();
                            subList.Add(device);
                            if (patterns.ContainsKey(pattern))
                            {
                                patterns[pattern] = subList;
                            }
                            else
                            {
                                patterns.Add(pattern, subList);
                            }

                        }
                    }
                }
            }
        }
        #endregion

        #region "Functions"
        /// <summary>
        ///  Main Resolver function : Returns Attribute dictionary for device resolved from useragent
        /// </summary>
        /// <param name="useragent">user-agent string to resolve</param>
        /// <returns>IDictionary(Of String, String)</returns>
        /// <remarks>-</remarks>
        public IDictionary<string, string> Map(string useragent)
        {
            if (string.IsNullOrEmpty(useragent))
            {
                return null;
            }
            Dictionary<string, IList<Device>> hits = new Dictionary<string, IList<Device>>();
            Device winner = null;
            string winnerStr = string.Empty;
            // The Split
            string[] parts = Regex.Split(useragent, " |-|_|/|\\\\|\\[|\\]|\\(|\\)|;");
            for (int i = 0; i <= parts.Length - 1; i++)
            {

                string pattern = "";
                int j = 0;
                while (j < 4 && (j + i) < parts.Length)
                {
                    if (!string.IsNullOrEmpty(parts[i + j]))
                    {
                        pattern += Util.Normalize(parts[i + j]);

                        if (patterns.ContainsKey(pattern))
                        {
                            hits[pattern] = patterns[pattern];
                        }
                    }
                    j += 1;
                }

            }
            foreach (string hit in hits.Keys)
            {
                foreach (Device device in hits[hit])
                {
                    if (device.Patterns.isValid(hits.Keys.ToList()))
                    {
                        if (winner != null)
                        {
                            if ("simple".Equals(winner.Type) && !"simple".Equals(device.Type))
                            {
                                winner = device;
                                winnerStr = hit;
                            }
                            else if (hit.Length > winnerStr.Length && (!"simple".Equals(device.Type) || device.Type.Equals(winner.Type)))
                            {
                                winner = device;
                                winnerStr = hit;
                            }
                        }
                        else
                        {
                            winner = device;
                            winnerStr = hit;
                        }
                    }
                }
            }
            if (winner != null)
            {
                return winner.Attributes;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}