﻿/**
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oddr.Models.Devices;
using Oddr.Models;
using System.Text.RegularExpressions;

namespace Oddr.Builders.Devices
{
    public class SymbianDeviceBuilder : OrderedTokenDeviceBuilder
    {
        private Dictionary<String, Device> devices;
        private Dictionary<String, Object> regexs = new Dictionary<String, Object>();

        public SymbianDeviceBuilder()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devices"></param>
        /// <exception cref="System.InvalidOperationException">Thrown when unable to find device with id in devices</exception>
        protected override void AfterOrderingCompleteInit(Dictionary<string, Device> devices)
        {
            this.devices = devices;
            foreach (Object devIdObj in orderedRules.Values)
            {
                String devId = (String)devIdObj;
                if (!devices.ContainsKey(devId))
                {
                    throw new InvalidOperationException("unable to find device with id: " + devId + "in devices");
                }
            }
        }

        public override void PutDevice(string deviceID, List<string> initProperties)
        {
            try
            {
                orderedRules.Add(initProperties[0], deviceID);
                regexs.Add(initProperties[0] + "_loose", Regex.Replace(initProperties[0], "[ _/-]", ".?"));
                regexs.Add(initProperties[0] + "_loose_icase_regex", new Regex(/*"(?i).*"*/".?>*" + Regex.Replace(initProperties[0], "[ _/-]", ".?") + ".*"));
            }
            catch (Exception ex)
            {
                //Console.WriteLine(this.GetType().FullName + " " + initProperties[0] + " " + deviceID + " " + ex.Message);
            }
        }

        public override bool CanBuild(UserAgent userAgent)
        {
            return userAgent.containsSymbian;
        }

        public override BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            List<Device> foundDevices = new List<Device>();

            foreach (string token in orderedRules.Keys)
            {
                Device d = ElaborateSymbianDeviceWithToken(userAgent, token);
                if (d != null)
                {
                    if (d.confidence > confidenceTreshold)
                    {
                        return d;

                    }
                    else
                    {
                        if (d.confidence > 0)
                        {
                            foundDevices.Add(d);
                        }
                    }
                }
            }

            if (foundDevices.Count > 0)
            {
                foundDevices.Sort();
                foundDevices.Reverse();
                return foundDevices[0];
            }
            return null;
        }

        private Device ElaborateSymbianDeviceWithToken(UserAgent userAgent, String token)
        {
            String originalToken = token;

            if (userAgent.mozillaPattern || userAgent.operaPattern)
            {
                int subtract = 0;
                String currentToken = token;

                String looseToken = (String)(regexs[token + "_loose"]);
                //String looseToken = Regex.Replace(token, "[ _/-]", ".?");

                Regex looseRegex = (Regex)(regexs[token + "_loose_icase_regex"]);
                //Regex looseRegex = new Regex(".*" + looseToken + ".*");

                if (!looseRegex.IsMatch(userAgent.completeUserAgent))
                {
                    return null;
                }

                Regex currentRegex = null;

                if (userAgent.operaPattern)
                {
                    subtract += 10;
                }
                for (int i = 0; i <= 1; i++)
                {
                    if (i == 1)
                    {
                        currentToken = looseToken;
                    }

                    currentRegex = new Regex(".?>*Series60.?(\\d+)\\.(\\d+).?" + currentToken + ".*");
                    if (userAgent.GetPatternElementsInside() != null && currentRegex.IsMatch(userAgent.GetPatternElementsInside()))
                    {
                        if (orderedRules.Contains(originalToken))
                        {
                            String deviceId = (String)orderedRules[originalToken];
                            Device retDevice = null;
                            if (devices.TryGetValue(deviceId, out retDevice))
                            {
                                retDevice = (Device)retDevice.Clone();
                                retDevice.confidence = (100 - subtract);
                                return retDevice;
                            }
                        }
                    }

                    currentRegex = new Regex(".?>*" + currentToken);
                    if (userAgent.GetPatternElementsPre() != null && currentRegex.IsMatch(userAgent.GetPatternElementsPre()))
                    {
                        if (orderedRules.Contains(originalToken))
                        {
                            String deviceId = (String)orderedRules[originalToken];
                            Device retDevice = null;
                            if (devices.TryGetValue(deviceId, out retDevice))
                            {
                                retDevice = (Device)retDevice.Clone();
                                retDevice.confidence = (95 - subtract);
                                return retDevice;
                            }
                        }
                    }

                    if (userAgent.GetPatternElementsInside() != null && currentRegex.IsMatch(userAgent.GetPatternElementsInside()))
                    {
                        if (orderedRules.Contains(originalToken))
                        {
                            String deviceId = (String)orderedRules[originalToken];
                            Device retDevice = null;
                            if (devices.TryGetValue(deviceId, out retDevice))
                            {
                                retDevice = (Device)retDevice.Clone();
                                retDevice.confidence = (90 - subtract);
                                return retDevice;
                            }
                        }
                    }

                    currentRegex = new Regex(".?>*" + currentToken + ".?;.*");
                    if (userAgent.GetPatternElementsInside() != null && currentRegex.IsMatch(userAgent.GetPatternElementsInside()))
                    {
                        if (orderedRules.Contains(originalToken))
                        {
                            String deviceId = (String)orderedRules[originalToken];
                            Device retDevice = null;
                            if (devices.TryGetValue(deviceId, out retDevice))
                            {
                                retDevice = (Device)retDevice.Clone();
                                retDevice.confidence = (90 - subtract);
                                return retDevice;
                            }
                        }
                    }

                    if (i == 1)
                    {
                        currentRegex = looseRegex;

                    }
                    else
                    {
                        currentRegex = new Regex(".?>*" + currentToken + ".*");
                    }

                    if (userAgent.GetPatternElementsInside() != null && currentRegex.IsMatch(userAgent.GetPatternElementsInside()))
                    {
                        if (orderedRules.Contains(originalToken))
                        {
                            String deviceId = (String)orderedRules[originalToken];
                            Device retDevice = null;
                            if (devices.TryGetValue(deviceId, out retDevice))
                            {
                                retDevice = (Device)retDevice.Clone();
                                retDevice.confidence = (80 - subtract);
                                return retDevice;
                            }
                        }
                    }

                    if (userAgent.GetPatternElementsPre() != null && currentRegex.IsMatch(userAgent.GetPatternElementsPre()))
                    {
                        if (orderedRules.Contains(originalToken))
                        {
                            String deviceId = (String)orderedRules[currentToken];
                            Device retDevice = null;
                            if (devices.TryGetValue(deviceId, out retDevice))
                            {
                                retDevice = (Device)retDevice.Clone();
                                retDevice.confidence = (80 - subtract);
                                return retDevice;
                            }
                        }
                    }

                    if (userAgent.GetPatternElementsPost() != null && currentRegex.IsMatch(userAgent.GetPatternElementsPost()))
                    {
                        if (orderedRules.Contains(originalToken))
                        {
                            String deviceId = (String)orderedRules[currentToken];
                            Device retDevice = null;
                            if (devices.TryGetValue(deviceId, out retDevice))
                            {
                                retDevice = (Device)retDevice.Clone();
                                retDevice.confidence = (60 - subtract);
                                return retDevice;
                            }
                        }
                    }
                    subtract += 20;
                }
            }
            else
            {
                String ua = Regex.Replace(userAgent.completeUserAgent, "SN[0-9]*", "");

                int subtract = 0;
                String currentToken = token;

                String looseToken = (String)(regexs[token + "_loose"]);
                //String looseToken = Regex.Replace(token, "[ _/-]", ".?");

                Regex looseRegex = (Regex)(regexs[token + "_loose_icase_regex"]);
                //Regex looseRegex = new Regex(".*" + looseToken + ".*");

                if (!looseRegex.IsMatch(userAgent.completeUserAgent))
                {
                    return null;
                }

                Regex currentRegex = null;

                for (int i = 0; i <= 1; i++)
                {
                    if (i == 1)
                    {
                        currentToken = looseToken;
                    }

                    currentRegex = new Regex(".?>*" + currentToken + ".*");
                    if (currentRegex.IsMatch(ua))
                    {
                        if (orderedRules.Contains(originalToken))
                        {
                            string deviceId = orderedRules[originalToken] as string;                            

                            if (devices.ContainsKey(deviceId))
                            {
                                Device retDevice = (Device)devices[deviceId].Clone();
                                retDevice.confidence = 100 - subtract;
                                return retDevice;
                            }
                        }
                    }

                    subtract += 20;
                }
            }

            return null;
        }
    }
}
