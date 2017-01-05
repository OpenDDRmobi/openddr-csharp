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
using Oddr.Models.Browsers;
using Oddr.Models;
using System.Text.RegularExpressions;

namespace Oddr.Builders.Browsers
{
    public class SafariBrowserBuilder : LayoutEngineBrowserBuilder
    {
        private const String SAFARI_VERSION_REGEXP = ".*Version/([0-9\\.]+).*";
        private static Regex safariVersionRegex = new Regex(SAFARI_VERSION_REGEXP);

        protected override Browser BuildBrowser(UserAgent userAgent, string layoutEngine, string layoutEngineVersion, int hintedWidth, int hintedHeight)
        {
            if (!(userAgent.mozillaPattern)) {
                return null;
            }

            int confidence = 60;
            Browser identified = new Browser();

            identified.SetVendor("Apple");
            identified.SetModel("Safari");
            identified.SetVersion("-");
            identified.majorRevision = "-";

            if (safariVersionRegex.IsMatch(userAgent.completeUserAgent)) {
                Match safariMatcher = safariVersionRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = safariMatcher.Groups;

                if (groups[1] != null && groups[1].Value.Trim().Length > 0) {
                    identified.SetVersion(groups[1].Value);

                    string versionFullString = groups[1].Value;
                    String[] version = versionFullString.Split(".".ToCharArray());

                    if (version.Length > 0) {
                        identified.majorRevision = version[0];
                        if (identified.majorRevision.Length == 0) {
                            identified.majorRevision = "1";
                        }
                    }

                    if (version.Length > 1) {
                        identified.minorRevision = version[1];
                        confidence += 10;
                    }

                    if (version.Length > 2) {
                        identified.microRevision = version[2];
                    }

                    if (version.Length > 3) {
                        identified.nanoRevision = version[3];
                    }
                }

            }

            if (layoutEngine != null) {
                identified.SetLayoutEngine(layoutEngine);
                identified.SetLayoutEngineVersion(layoutEngineVersion);
                if (layoutEngine.Equals(LayoutEngineBrowserBuilder.GECKO)) {
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
            return (userAgent.completeUserAgent.Contains("Safari") && !userAgent.completeUserAgent.Contains("Mobile"));
        }
    }
}
