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
using System.Text.RegularExpressions;
using Oddr.Models;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Oddr.Builders.Devices
{
    public abstract class OrderedTokenDeviceBuilder : IDeviceBuilder
    {
        protected OrderedDictionary orderedRules;

        /// <exception cref="System.InvalidOperationException">Thrown when unable to find device with id in devices</exception>
        protected abstract void AfterOrderingCompleteInit(Dictionary<String, Device> devices);

        public OrderedTokenDeviceBuilder()
        {
            orderedRules = new OrderedDictionary();
        }

        public void CompleteInit(Dictionary<string, Device> devices)
        {
            
            Dictionary<String, Object> tmp = new Dictionary<String, Object>();
            List<String> keys = new List<String>();
            foreach (string key in orderedRules.Keys)
            {
                keys.Add(key);
            }

            keys.Sort(new OrderedTokenDeviceComparer());

            foreach (String str in keys)
            {
                tmp.Add(str, orderedRules[str]);
            }
            List<String> keysOrdered = new List<String>();

            orderedRules = new OrderedDictionary();

            while (keys.Count > 0)
            {
                bool found = false;

                foreach (string k1 in keys)
                {
                    Regex k1Regex = new Regex(".*" + k1 + ".*");

                    foreach (string k2 in keys)
                    {
                        if ((!k1.Equals(k2)) && k1Regex.IsMatch(k2))
                        //if ((!k1.Equals(k2)) && Regex.IsMatch(k2, ".*" + k1 + ".*"))
                        //if ((!k1.Equals(k2)) && k2.Contains(k1))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        keysOrdered.Add(k1);
                        keys.Remove(k1);
                        break;
                    }
                }

                if (!found)
                {
                    continue;
                }
                int max = 0;
                int idx = -1;
                for (int i = 0; i < keys.Count; i++)
                {
                    String str = keys[i];
                    if (str.Length > max)
                    {
                        max = str.Length;
                        idx = i;
                    }
                }

                if (idx >= 0)
                {
                    keysOrdered.Add(keys[idx]);
                    keys.RemoveAt(idx);
                }
            }

            foreach (String key in keysOrdered)
            {
                orderedRules.Add(key, tmp[key]);
                tmp.Remove(key);
            }

            AfterOrderingCompleteInit(devices);
        }

        public abstract void PutDevice(string deviceID, List<string> initProperties);

        public abstract bool CanBuild(UserAgent userAgent);

        public abstract BuiltObject Build(UserAgent userAgent, int confidenceTreshold);
    }

    public class OrderedTokenDeviceComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return y.Length - x.Length;
        }
    }
}
