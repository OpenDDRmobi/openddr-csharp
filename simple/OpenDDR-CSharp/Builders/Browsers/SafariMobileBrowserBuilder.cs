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
using System.Text.RegularExpressions;
using Oddr.Models;
using Oddr.Models.Browsers;

namespace Oddr.Builders.Browsers
{
    public class SafariMobileBrowserBuilder : LayoutEngineBrowserBuilder
    {
        private const string VERSION_REGEXP = ".*Version/([0-9\\.]+).*?";
        private const string SAFARI_REGEXP = ".*Safari/([0-9\\.]+).*?";
        private static Regex versionRegex = new Regex(VERSION_REGEXP);
        private static Regex safariRegex = new Regex(SAFARI_REGEXP);


        protected override Browser BuildBrowser(UserAgent userAgent, string layoutEngine, string layoutEngineVersion, int hintedWidth, int hintedHeight)
        {
            if (!userAgent.containsIOSDevices)
            {
                return null;
            }

            int confidence = 70;
            Browser identified = new Browser();

            identified.SetVendor("Apple");
            identified.SetModel("Mobile Safari");

            if (versionRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match versionMatcher = versionRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = versionMatcher.Groups;

                if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                {
                    identified.SetVersion(groups[1].Value);
                    string versionFullString = groups[1].Value;
                    String[] version = versionFullString.Split(".".ToCharArray());

                    if (version.Length > 0)
                    {
                        identified.majorRevision = version[0];
                    }

                    if (version.Length > 1)
                    {
                        identified.minorRevision = version[1];
                        confidence += 10;
                    }

                    if (version.Length > 2)
                    {
                        identified.microRevision = version[2];
                    }

                    if (version.Length > 3)
                    {
                        identified.nanoRevision = version[3];
                    }
                }

            }
            else
            {
                //fallback version
                identified.SetVersion("1.0");
                identified.majorRevision = "1";
            }

            if (layoutEngine != null)
            {
                identified.SetLayoutEngine(layoutEngine);
                identified.SetLayoutEngineVersion(layoutEngineVersion);
                if (layoutEngine.Equals(LayoutEngineBrowserBuilder.APPLEWEBKIT))
                {
                    confidence += 10;
                }
            }

            if (safariRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match safariMatcher = safariRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = safariMatcher.Groups;

                if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                {
                    identified.SetReferenceBrowser("Safari");
                    identified.SetReferenceBrowserVersion(groups[1].Value);
                    confidence += 10;
                }
            }

            identified.SetDisplayWidth(hintedWidth);
            identified.SetDisplayHeight(hintedHeight);
            identified.confidence = confidence;

            return identified;
        }

        public override bool CanBuild(UserAgent userAgent)
        {
            return userAgent.containsIOSDevices;
        }
    }
}
