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
    public class OperaBrowserBuilder : LayoutEngineBrowserBuilder
    {
        private const String OPERAMINI_VERSION_REGEXP = "Opera Mobi/(.*)";
        private const String OPERA_VERSION_REGEXP = ".* Opera ([0-9\\.]+).*";
        private static Regex operaMiniVersionRegex = new Regex(OPERAMINI_VERSION_REGEXP, RegexOptions.Compiled);
        private static Regex operaVersionRegex = new Regex(OPERA_VERSION_REGEXP, RegexOptions.Compiled);

        protected override Browser BuildBrowser(UserAgent userAgent, string layoutEngine, string layoutEngineVersion, int hintedWidth, int hintedHeight)
        {
            if ((!userAgent.operaPattern || userAgent.operaVersion == null || userAgent.operaVersion.Length == 0) && (!operaVersionRegex.IsMatch(userAgent.completeUserAgent)))
            {
                return null;
            }

            int confidence = 60;
            Browser identified = new Browser();

            identified.SetVendor("Opera");
            if (userAgent.completeUserAgent.Contains("Mobi"))
            {
                identified.SetModel("Opera Mobile");
                confidence += 10;

            }
            else if (userAgent.completeUserAgent.Contains("Tablet"))
            {
                identified.SetModel("Opera Tablet");

            }
            else
            {
                identified.SetModel("Opera");
            }

            if (userAgent.operaVersion != null)
            {
                identified.SetVersion(userAgent.operaVersion);
            }
            else
            {
                if (operaVersionRegex.IsMatch(userAgent.completeUserAgent))
                {
                    Match operaMatcher = operaVersionRegex.Match(userAgent.completeUserAgent);
                    GroupCollection groups = operaMatcher.Groups;

                    if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                    {
                        identified.SetVersion(groups[1].Value);
                    }
                }
            }

            String[] version = identified.GetVersion().Split(".".ToCharArray());

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

            if (layoutEngine != null)
            {
                identified.SetLayoutEngine(layoutEngine);
                identified.SetLayoutEngineVersion(layoutEngineVersion);
                if (layoutEngine.Equals(LayoutEngineBrowserBuilder.PRESTO))
                {
                    confidence += 10;
                }
            }

            if (userAgent.GetPatternElementsInside() != null)
            {
                String[] inside = userAgent.GetPatternElementsInside().Split(";".ToCharArray());
                foreach (String token in inside)
                {
                    String element = token.Trim();

                    if (operaMiniVersionRegex.IsMatch(element))
                    {
                        Match miniMatcher = operaMiniVersionRegex.Match(element);
                        GroupCollection groups = miniMatcher.Groups;

                        if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                        {
                            identified.SetReferenceBrowser("Opera Mobi");
                            identified.SetReferenceBrowserVersion(groups[1].Value);
                            confidence += 10;
                            break;
                        }
                    }
                }
            }

            identified.SetDisplayWidth(hintedWidth);
            identified.SetDisplayHeight(hintedHeight);
            identified.confidence = confidence;

            return identified;
        }

        public override bool CanBuild(UserAgent userAgent)
        {
            return ((userAgent.operaPattern || operaVersionRegex.IsMatch(userAgent.completeUserAgent)) && (!userAgent.completeUserAgent.Contains("Opera Mini")));
        }
    }
}
