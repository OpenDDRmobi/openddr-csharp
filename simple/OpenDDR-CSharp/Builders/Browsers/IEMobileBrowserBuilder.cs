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
using Oddr.Models;
using Oddr.Models.Browsers;

namespace Oddr.Builders.Browsers
{
    class IEMobileBrowserBuilder : LayoutEngineBrowserBuilder
    {
        private const string VERSION_REGEXP = ".*[^MS]IEMobile.([0-9\\.]+).*?";
        private const string MSIE_VERSION_REGEXP = ".*MSIE.([0-9\\.]+).*";
        private const string MSIEMOBILE_VERSION_REGEXP = ".*MSIEMobile.([0-9\\.]+).*";
        private static Regex versionRegex = new Regex(VERSION_REGEXP, RegexOptions.Compiled);
        private static Regex msieVersionRegex = new Regex(MSIE_VERSION_REGEXP, RegexOptions.Compiled);
        private static Regex msieMobileVersionRegex = new Regex(MSIEMOBILE_VERSION_REGEXP, RegexOptions.Compiled);

        private const string WINDOWS_CE_PHONE_REGEXP = ".*Windows.?(?:(?:CE)|(?:Phone)).*";
        private static Regex windowsCePhoneRegex = new Regex(WINDOWS_CE_PHONE_REGEXP, RegexOptions.Compiled);

        protected override Browser BuildBrowser(UserAgent userAgent, string layoutEngine, string layoutEngineVersion, int hintedWidth, int hintedHeight)
        {
            if (!userAgent.containsWindowsPhone || !(windowsCePhoneRegex.IsMatch(userAgent.completeUserAgent)))
            {
                return null;
            }

            int confidence = 40;
            Browser identified = new Browser();

            identified.SetVendor("Microsoft");
            identified.SetModel("IEMobile");

            if (userAgent.completeUserAgent.Contains("MSIEMobile"))
            {
                confidence += 10;
            }

            if (userAgent.mozillaPattern)
            {
                confidence += 10;
            }

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

            if (msieVersionRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match msieMatcher = msieVersionRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = msieMatcher.Groups;

                if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                {
                    identified.SetReferenceBrowser("MSIE");
                    identified.SetReferenceBrowserVersion(groups[1].Value);
                    confidence += 10;
                }
            }

            if (msieMobileVersionRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match msieMobileMatcher = msieMobileVersionRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = msieMobileMatcher.Groups;

                if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                {
                    identified.SetLayoutEngine("MSIEMobile");
                    identified.SetLayoutEngineVersion(groups[1].Value);
                    confidence += 10;
                }
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

            identified.SetDisplayWidth(hintedWidth);
            identified.SetDisplayHeight(hintedHeight);
            identified.confidence = confidence;

            return identified;
        }

        public override bool CanBuild(UserAgent userAgent)
        {
            return userAgent.containsWindowsPhone;
        }
    }
}
