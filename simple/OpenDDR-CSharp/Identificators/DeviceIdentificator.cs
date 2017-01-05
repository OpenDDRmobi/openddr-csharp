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
using W3c.Ddr.Simple;
using Oddr.Models;
using Oddr.Models.Devices;
using Oddr.Builders.Devices;

namespace Oddr.Identificators
{
    public class DeviceIdentificator : IIdentificator
    {
        private IDeviceBuilder[] builders;
        private Dictionary<String, Device> devices;

        public DeviceIdentificator(IDeviceBuilder[] builders, Dictionary<String, Device> devices)
        {
            this.builders = builders;
            this.devices = devices;
        }

        public BuiltObject Get(string userAgent, int confidenceTreshold)
        {
            return Get(UserAgentFactory.newUserAgent(userAgent), confidenceTreshold);
        }

        //XXX: to be refined, this should NOT be the main entry point, we should use a set of evidence derivation
        public BuiltObject Get(IEvidence evdnc, int threshold)
        {
            UserAgent ua = UserAgentFactory.newDeviceUserAgent(evdnc);
            if (ua != null)
            {
                return Get(ua, threshold);
            }
            return null;
        }

        public BuiltObject Get(UserAgent userAgent, int confidenceTreshold)
        {
            List<Device> foundDevices = new List<Device>();
            Device foundDevice = null;
            foreach (IDeviceBuilder deviceBuilder in builders)
            {
                if (deviceBuilder.CanBuild(userAgent))
                {
                    Device device = (Device)deviceBuilder.Build(userAgent, confidenceTreshold);
                    if (device != null)
                    {
                        String parentId = device.parentId;
                        Device parentDevice = null;
                        while (!"root".Equals(parentId))
                        {
                            if (devices.TryGetValue(parentId, out parentDevice))
                            {
                                foreach (KeyValuePair<string, string> entry in parentDevice.properties)
                                {
                                    if (!device.ContainsProperty(entry.Key))
                                    {
                                        device.PutProperty(entry.Key, entry.Value);
                                    }
                                }
                                parentId = parentDevice.parentId;
                            }
                        }

                        foundDevices.Add(device);
                        if (device.confidence >= confidenceTreshold)
                        {
                            foundDevice = device;
                            break;
                        }
                    }
                }
            }

            if (foundDevice != null)
            {
                return foundDevice;

            }
            else
            {
                if (foundDevices.Count > 0)
                {
                    foundDevices.Sort();
                    foundDevices.Reverse();
                    return foundDevices[0];
                }

                return null;
            }
        }

        public void CompleteInit()
        {
            foreach (IDeviceBuilder deviceBuilder in builders)
            {
                try
                {
                    deviceBuilder.CompleteInit(devices);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(this.GetType().FullName + " " + ex.Message);
                }
            }
        }
    }
}
