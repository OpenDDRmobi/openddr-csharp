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
using System.Runtime.CompilerServices;
using Oddr.Models.Browsers;

namespace Oddr.Builders.Browsers
{
    public class DefaultBrowserBuilder : IBuilder
    {
        private IBuilder[] builders;
        private static volatile DefaultBrowserBuilder instance;
        private static object syncRoot = new Object();

        private DefaultBrowserBuilder()
        {
            builders = new IBuilder[]
            {
                new OperaMiniBrowserBuilder(),
                new ChromeMobileBrowserBuilder(),
                new FennecBrowserBuilder(),
                new SafariMobileBrowserBuilder(),
                new SilkBrowserBuilder(),
                new AndroidMobileBrowserBuilder(),
                new NetFrontBrowserBuilder(),
                new UPBrowserBuilder(),
                new OpenWaveBrowserBuilder(),
                new SEMCBrowserBuilder(),
                new DolfinBrowserBuilder(),
                new JasmineBrowserBuilder(),
                new PolarisBrowserBuilder(),
                new ObigoBrowserBuilder(),
                new OperaBrowserBuilder(),
                new IEMobileBrowserBuilder(),
                new NokiaBrowserBuilder(),
                new BlackBerryBrowserBuilder(),
                new WebOsBrowserBuilder(),
                new InternetExplorerBrowserBuilder(),
                new ChromeBrowserBuilder(),
                new FirefoxBrowserBuilder(),
                new SafariBrowserBuilder(),
                new KonquerorBrowserBuilder(),
            };
        }

        public static DefaultBrowserBuilder Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new DefaultBrowserBuilder();
                        }
                    }
                }

                return instance;
            }
        }

        public bool CanBuild(UserAgent userAgent)
        {
            foreach (IBuilder browserBuilder in builders)
            {
                if (browserBuilder.CanBuild(userAgent))
                {
                    return true;
                }
            }
            return false;
        }

        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            List<Browser> founds = new List<Browser>();
            Browser found = null;
            foreach (IBuilder builder in builders)
            {
                if (builder.CanBuild(userAgent))
                {
                    Browser builded = (Browser)builder.Build(userAgent, confidenceTreshold);
                    if (builded != null)
                    {
                        founds.Add(builded);
                        if (builded.confidence >= confidenceTreshold)
                        {
                            found = builded;
                            break;
                        }
                    }
                }
            }

            if (found != null)
            {
                return found;

            }
            else
            {
                if (founds.Count == 0)
                {
                    return null;
                }

                founds.Sort();
                founds.Reverse();

                return founds[0];
            }
        }
    }
}
