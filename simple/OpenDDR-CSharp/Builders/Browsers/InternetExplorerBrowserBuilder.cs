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
using System.Text.RegularExpressions;
using Oddr.Models.Browsers;
using Oddr.Models;

namespace Oddr.Builders.Browsers
{
    public class InternetExplorerBrowserBuilder : LayoutEngineBrowserBuilder
    {
        private const String MSIE_VERSION_REGEXP = ".*MSIE.([0-9\\.b]+).*";
        private const String DOT_NET_CLR_REGEXP = ".*\\.NET.CLR.*";
        private static Regex msieVersionRegex = new Regex(MSIE_VERSION_REGEXP, RegexOptions.Compiled);
        private static Regex dotNetClrRegex = new Regex(DOT_NET_CLR_REGEXP, RegexOptions.Compiled);

        private const string WINDOWS_CE_PHONE = ".*Windows.?(?:(?:CE)|(?:Phone)).*";
        private static Regex windowsCePhoneRegex = new Regex(WINDOWS_CE_PHONE, RegexOptions.Compiled);

        protected override Browser BuildBrowser(UserAgent userAgent, string layoutEngine, string layoutEngineVersion, int hintedWidth, int hintedHeight)
        {
            if (!userAgent.containsMSIE || !userAgent.mozillaPattern || windowsCePhoneRegex.IsMatch(userAgent.completeUserAgent))
            {
                return null;
            }

            int confidence = 60;
            Browser identified = new Browser();

            identified.SetVendor("Microsoft");
            identified.SetModel("Internet Explorer");

            if (msieVersionRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match msieMatcher = msieVersionRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = msieMatcher.Groups;

                if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                {
                    identified.SetVersion(groups[1].Value);

                    string versionFullString = groups[1].Value;
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
                if (layoutEngine.Equals(LayoutEngineBrowserBuilder.TRIDENT))
                {
                    confidence += 10;
                }
            }

            if (dotNetClrRegex.IsMatch(userAgent.completeUserAgent))
            {
                confidence += 10;
            }

            identified.SetDisplayWidth(hintedWidth);
            identified.SetDisplayHeight(hintedHeight);
            identified.confidence = confidence;

            return identified;
        }

        public override bool CanBuild(UserAgent userAgent)
        {
            return userAgent.containsMSIE;
        }
    }
}
