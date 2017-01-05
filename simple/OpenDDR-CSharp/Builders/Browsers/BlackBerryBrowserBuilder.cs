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
    public class BlackBerryBrowserBuilder : LayoutEngineBrowserBuilder
    {
        private const String BLACKBERRY_VERSION_REGEXP = ".*(?:(?:Version)|(?:[Bb]lack.?[Bb]erry.?(?:[0-9a-z]+)))/([0-9\\.]+).*";//"(?:.*?Version.?([0-9\\.]+).*)|(?:.*?[Bb]lack.?[Bb]erry(?:\\d+)/([0-9\\.]+).*)";
        private const String SAFARI_VERSION_REGEXP = ".*Safari/([0-9\\.]+).*";
        private static Regex blackberryVersionRegex = new Regex(BLACKBERRY_VERSION_REGEXP, RegexOptions.Compiled);
        private static Regex safariVersionRegex = new Regex(SAFARI_VERSION_REGEXP, RegexOptions.Compiled);

        protected override Browser BuildBrowser(UserAgent userAgent, string layoutEngine, string layoutEngineVersion, int hintedWidth, int hintedHeight)
        {
            int confidence = 50;
            Browser identified = new Browser();

            identified.SetVendor("RIM");
            identified.SetModel("BlackBerry");
            identified.SetVersion("-");
            identified.majorRevision = "-";

            if (blackberryVersionRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match blackberryBrowserMatcher = blackberryVersionRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = blackberryBrowserMatcher.Groups;
                if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                {
                    String totalVersion = groups[1].Value;

                    if (totalVersion.Length > 0)
                    {
                        identified.SetVersion(totalVersion);
                        String[] version = totalVersion.Split(".".ToCharArray());

                        if (version.Length > 0)
                        {
                            identified.majorRevision = version[0];
                            if (identified.majorRevision.Length == 0)
                            {
                                identified.majorRevision = "1";
                            }
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

            if (safariVersionRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match safariMatcher = safariVersionRegex.Match(userAgent.completeUserAgent);
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
            return userAgent.completeUserAgent.Contains("BlackBerry");
        }
    }
}
