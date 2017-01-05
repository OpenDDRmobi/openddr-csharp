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
    public class ChromeBrowserBuilder : LayoutEngineBrowserBuilder
    {
        private const String CHROME_VERSION_REGEXP = ".*Chrome.([0-9a-z\\.b]+).*";
        private const String SAFARI_REGEXP = ".*Safari/([0-9\\.]+).*?";
        private static Regex chromeVersionRegex = new Regex(CHROME_VERSION_REGEXP, RegexOptions.Compiled);
        private static Regex safariRegex = new Regex(SAFARI_REGEXP, RegexOptions.Compiled);

        protected override Browser BuildBrowser(UserAgent userAgent, string layoutEngine, string layoutEngineVersion, int hintedWidth, int hintedHeight)
        {
            if (!(userAgent.mozillaPattern))
            {
                return null;
            }

            int confidence = 60;
            Browser identified = new Browser();

            identified.SetVendor("Google");
            identified.SetModel("Chrome");
            identified.SetVersion("-");
            identified.majorRevision = "-";

            if (chromeVersionRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match chromeMatcher = chromeVersionRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = chromeMatcher.Groups;
                if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                {
                    string versionFullString = groups[1].Value;
                    identified.SetVersion(versionFullString);
                    String[] version = versionFullString.Split(".".ToCharArray());

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
                if (layoutEngine.Equals(LayoutEngineBrowserBuilder.KHML))
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
            return userAgent.completeUserAgent.Contains("Chrome");
        }
    }
}
