/**
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
using System.Collections.Specialized;
using Oddr.Models.Devices;
using Oddr.Models;
using System.Text.RegularExpressions;

namespace Oddr.Builders.Devices
{
    public class SimpleDeviceBuilder : IDeviceBuilder
    {
        private OrderedDictionary simpleTokenMap;
        private Dictionary<String, Device> devices;

        public SimpleDeviceBuilder()
            : base()
        {
            simpleTokenMap = new OrderedDictionary();
        }

        public void PutDevice(string deviceID, List<string> initProperties)
        {
            foreach (String token in initProperties)
            {
                simpleTokenMap[token] = deviceID; // override duplicate
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devices"></param>
        /// <exception cref="System.InvalidOperationException">Thrown when unable to find device with id in devices</exception>
        public void CompleteInit(Dictionary<string, Device> devices)
        {
            this.devices = devices;

            foreach (String deviceID in simpleTokenMap.Values)
            {
                if (!devices.ContainsKey(deviceID))
                {
                    throw new InvalidOperationException("unable to find device with id: " + deviceID + "in devices");
                }
            }
        }

        public bool CanBuild(UserAgent userAgent)
        {
            foreach (String token in simpleTokenMap.Keys)
            {
                Regex tokenRegex = new Regex(/*"(?i).*"*/".*" + Regex.Escape(token) + ".*", RegexOptions.IgnoreCase);
                if (tokenRegex.IsMatch(userAgent.completeUserAgent))
                {
                    return true;
                }
            }
            return false;
        }

        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            foreach (string token in simpleTokenMap.Keys)
            {
                Regex tokenRegex = new Regex(/*"(?i).*"*/".*" + Regex.Escape(token) + ".*", RegexOptions.IgnoreCase);
                if (tokenRegex.IsMatch(userAgent.completeUserAgent) && simpleTokenMap.Contains(token))
                {
                    String desktopDeviceId = (string)simpleTokenMap[token];
                    Device device = null;
                    if (devices.TryGetValue(desktopDeviceId, out device))
                    {
                        return device;
                    }
                }
            }
            return null;
        }
    }
}
