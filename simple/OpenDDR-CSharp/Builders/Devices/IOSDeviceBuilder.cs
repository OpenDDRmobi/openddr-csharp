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
using Oddr.Models.Devices;
using Oddr.Models;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace Oddr.Builders.Devices
{
    public class IOSDeviceBuilder : IDeviceBuilder
    {
        private OrderedDictionary iOSDevices;
        private Dictionary<String, Device> devices;

        public IOSDeviceBuilder()
            : base()
        {
            iOSDevices = new OrderedDictionary();
        }

        public void PutDevice(string deviceID, List<string> initProperties)
        {
            try
            {
                iOSDevices.Add(initProperties[0], deviceID);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(this.GetType().FullName + " " + initProperties[0] + " " + deviceID + " " + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devices"></param>
        /// <exception cref="System.InvalidOperationException">Thrown when unable to find device with id in devices</exception>
        public void CompleteInit(Dictionary<string, Device> devices)
        {
            String global = "iPhone";
            if (iOSDevices.Contains(global)) {
                String iphone = (string)iOSDevices[global];
                iOSDevices.Remove(global);
                iOSDevices.Add(global, iphone);
            }

            this.devices = devices;

            foreach (String deviceID in iOSDevices.Values) {
                if (!devices.ContainsKey(deviceID)) {
                    throw new InvalidOperationException("unable to find device with id: " + deviceID + " in devices");
                }
            }
        }

        public bool CanBuild(UserAgent userAgent)
        {
            return userAgent.containsIOSDevices && (!userAgent.containsAndroid) && (!userAgent.containsWindowsPhone);
        }

        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            foreach (string token in iOSDevices.Keys)
            {
                Regex tokenRegex = new Regex(".*" + token + ".*");
                if (tokenRegex.IsMatch(userAgent.completeUserAgent))
                {
                    if (iOSDevices.Contains(token))
                    {
                        String iosDeviceID = (string)iOSDevices[token];
                        Device retDevice = null;
                        if (devices.TryGetValue(iosDeviceID, out retDevice))
                        {
                            retDevice = (Device)retDevice.Clone();
                            retDevice.confidence = 90;
                            return retDevice;
                        }
                    }
                }
            }
            return null;
        }
    }
}
