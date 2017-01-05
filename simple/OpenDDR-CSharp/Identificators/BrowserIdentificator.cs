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
using Oddr.Models;
using W3c.Ddr.Simple;
using Oddr.Models.Browsers;
using Oddr.Builders;
using log4net;
using log4net.Config;

namespace Oddr.Identificators
{
    public class BrowserIdentificator : IIdentificator
    {
        protected static readonly ILog logger = LogManager.GetLogger(typeof(BrowserIdentificator));
        private IBuilder[] builders;
        private Dictionary<String, Browser> browserCapabilities;

        public BrowserIdentificator(IBuilder[] builders, Dictionary<String, Browser> browserCapabilities)
        {
            this.builders = builders;
            this.browserCapabilities = browserCapabilities;
        }

        public BuiltObject Get(string userAgent, int confidenceTreshold)
        {
            UserAgent ua = UserAgentFactory.newUserAgent(userAgent);
            return Get(ua, confidenceTreshold);
        }

        //XXX: to be refined, this should NOT be the main entry point, we should use a set of evidence derivation
        public BuiltObject Get(IEvidence evdnc, int threshold)
        {
            UserAgent ua = UserAgentFactory.newBrowserUserAgent(evdnc);

            if (ua != null)
            {
                return Get(ua, threshold);
            }

            return null;
        }

        public BuiltObject Get(UserAgent userAgent, int confidenceTreshold)
        {
            foreach (IBuilder builder in builders)
            {
                if (builder.CanBuild(userAgent))
                {
                    Browser browser = (Browser)builder.Build(userAgent, confidenceTreshold);
                    if (browser != null)
                    {
                        if (browserCapabilities != null)
                        {
                            String bestID = GetClosestKnownBrowserID(browser.GetId());
                            if (bestID != null)
                            {
                                Browser b = null;
                                if (browserCapabilities.TryGetValue(bestID, out b))
                                {
                                    browser.PutPropertiesMap(b.properties);
                                    if (!bestID.Equals(browser.GetId()))
                                    {
                                        browser.confidence = (browser.confidence - 15);
                                    }

                                }
                            }
                        }
                        return browser;
                    }
                }
            }

            return null;
        }

        private String GetClosestKnownBrowserID(String actualBrowserID)
        {
            XmlConfigurator.Configure();

            if (actualBrowserID == null)
            {
                return null;
            }

            int idx = actualBrowserID.IndexOf(".");

            if (idx < 0)
            {
                logger.Error("SHOULD NOT BE HERE, PLEASE CHECK BROWSER DOCUMENT(1)");
                logger.Debug(actualBrowserID);
                return null;

            }
            else
            {
                idx++;
            }
            idx = actualBrowserID.IndexOf(".", idx);

            if (idx < 0)
            {
                logger.Error("SHOULD NOT BE HERE, PLEASE CHECK BROWSER DOCUMENT(2)" + idx);
                logger.Debug(actualBrowserID);
                return null;

            }
            else
            {
                idx++;
            }

            String bestID = null;
            foreach (String listBrowserID in browserCapabilities.Keys)
            {
                if (listBrowserID.Equals(actualBrowserID))
                {
                    return actualBrowserID;
                }

                if (listBrowserID.Length > idx && listBrowserID.Substring(0, idx).Equals(actualBrowserID.Substring(0, idx)))
                {
                    if (listBrowserID.CompareTo(actualBrowserID) <= 0)
                    {
                        bestID = listBrowserID;
                    }
                }
            }

            return bestID;
        }

        public void CompleteInit()
        {
            //does nothing
        }
    }
}
