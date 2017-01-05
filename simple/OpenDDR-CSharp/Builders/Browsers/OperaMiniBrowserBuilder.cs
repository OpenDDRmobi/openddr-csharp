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
    public class OperaMiniBrowserBuilder : LayoutEngineBrowserBuilder
    {
        private const String VERSION_REGEXP = ".*?Opera Mini/(?:att/)?v?((\\d+)\\.(\\d+)(?:\\.(\\d+))?(?:\\.(\\d+))?).*?";
        private const String BUILD_REGEXP = ".*?Opera Mini/(?:att/)?v?.*?/(.*?);.*";
        private static Regex versionRegex = new Regex(VERSION_REGEXP, RegexOptions.Compiled);
        private static Regex buildRegex = new Regex(BUILD_REGEXP, RegexOptions.Compiled);

        protected override Browser BuildBrowser(UserAgent userAgent, string layoutEngine, string layoutEngineVersion, int hintedWidth, int hintedHeight)
        {
            if (!versionRegex.IsMatch(userAgent.completeUserAgent))
            {
                return null;
            }

            Match versionMatcher = versionRegex.Match(userAgent.completeUserAgent);
            GroupCollection groups = versionMatcher.Groups;

            int confidence = 60;
            Browser identified = new Browser();

            identified.SetVendor("Opera");
            identified.SetModel("Opera Mini");

            if (groups[1] != null && groups[1].Value.Trim().Length > 0)
            {
                identified.SetVersion(groups[1].Value);
            }
            if (groups[2] != null && groups[2].Value.Trim().Length > 0)
            {
                identified.majorRevision = groups[2].Value;
            }
            if (groups[3] != null && groups[3].Value.Trim().Length > 0)
            {
                identified.minorRevision = groups[3].Value;
            }
            if (groups[4] != null && groups[4].Value.Trim().Length > 0)
            {
                identified.microRevision = groups[4].Value;
            }
            if (groups[5] != null && groups[5].Value.Trim().Length > 0)
            {
                identified.nanoRevision = groups[5].Value;
            }

            if (userAgent.operaPattern && userAgent.operaVersion != null)
            {
                identified.SetReferenceBrowser("Opera");
                identified.SetReferenceBrowserVersion(userAgent.operaVersion);
                confidence += 20;
            }

            if (buildRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match buildMatcher = buildRegex.Match(userAgent.completeUserAgent);
                GroupCollection buildGroups = buildMatcher.Groups;

                if (buildGroups[1] != null && buildGroups[1].Value.Trim().Length > 0)
                {
                    identified.SetBuild(buildGroups[1].Value);
                    confidence += 10;
                }
            }

            if (layoutEngine != null)
            {
                identified.SetLayoutEngine(layoutEngine);
                identified.SetLayoutEngineVersion(layoutEngineVersion);
                if (layoutEngine.Equals(LayoutEngineBrowserBuilder.PRESTO))
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
            return userAgent.completeUserAgent.Contains("Opera Mini");
        }
    }
}
