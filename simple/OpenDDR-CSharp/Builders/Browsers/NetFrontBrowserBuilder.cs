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
    public class NetFrontBrowserBuilder : LayoutEngineBrowserBuilder
    {
        private const String VERSION_REGEXP = ".*?(?:(?:Net[Ff]ront)|(?:ACS-NF)|(?:NF-Browser))[/ ]?[/ ]?(?:WAP)?([0-9\\.]+).*?";
        private static Regex versionRegex = new Regex(VERSION_REGEXP, RegexOptions.Compiled);

        private const string NETFRONT_REGEXP = /*"(?i).*netfront.*"*/".*netfront.*";
        private static Regex netfrontRegex = new Regex(NETFRONT_REGEXP, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected override Browser BuildBrowser(UserAgent userAgent, string layoutEngine, string layoutEngineVersion, int hintedWidth, int hintedHeight)
        {
            String version = null;

            if (!versionRegex.IsMatch(userAgent.completeUserAgent))
            {
                return null;

            }
            else
            {
                Match versionMatcher = versionRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = versionMatcher.Groups;

                if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                {
                    version = groups[1].Value;
                }
            }

            int confidence = 60;
            Browser identified = new Browser();

            identified.SetVendor("Access");
            identified.SetModel("NetFront");
            identified.SetVersion(version);
            String[] versionEl = version.Split(".".ToCharArray());

            if (versionEl.Length > 0)
            {
                identified.majorRevision = versionEl[0];
            }

            if (versionEl.Length > 1)
            {
                identified.minorRevision = versionEl[1];
                confidence += 10;
            }

            if (versionEl.Length > 2)
            {
                identified.microRevision = versionEl[2];
            }

            if (versionEl.Length > 3)
            {
                identified.nanoRevision = versionEl[3];
            }

            if (layoutEngine != null)
            {
                identified.SetLayoutEngine(layoutEngine);
                identified.SetLayoutEngineVersion(layoutEngineVersion);
            }

            identified.SetDisplayWidth(hintedWidth);
            identified.SetDisplayHeight(hintedHeight);
            identified.confidence = confidence;

            return identified;
        }

        public override bool CanBuild(UserAgent userAgent)
        {
            if (netfrontRegex.IsMatch(userAgent.completeUserAgent) || userAgent.completeUserAgent.Contains("ACS-NF") || userAgent.completeUserAgent.Contains("NF-Browser"))
            {
                return true;
            }
            return false;
        }
    }
}
