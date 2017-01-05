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
using System.Text.RegularExpressions;
using OSModel = Oddr.Models.OS;

namespace Oddr.Builders.OS.Mozilla
{
    public class WinCEMozillaSubBuilder : IBuilder
    {
        private const String VERSION_REGEXP = ".*Windows.?CE.?((\\d+)?\\.?(\\d+)?\\.?(\\d+)?).*";
        private const String VERSION_MSIE_IEMOBILE = "(?:.*(?:MSIE).?(\\d+)\\.(\\d+).*)|(?:.*IEMobile.?(\\d+)\\.(\\d+).*)";
        private Regex versionRegex = new Regex(VERSION_REGEXP);
        private Regex versionMsieRegex = new Regex(VERSION_MSIE_IEMOBILE);

        public bool CanBuild(UserAgent userAgent)
        {
            if (userAgent.containsWindowsPhone)
            {
                Regex winCeRegex = new Regex(".*Windows.?CE.*");
                if (winCeRegex.IsMatch(userAgent.GetPatternElementsInside()))
                {
                    return true;
                }
            }
            return false;
        }

        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            OSModel.OperatingSystem model = new OSModel.OperatingSystem();
            model.majorRevision = "1";
            model.SetVendor("Microsoft");
            model.SetModel("Windows Phone");
            model.confidence = 40;

            string patternElementsInside = userAgent.GetPatternElementsInside();
            String[] splittedTokens = patternElementsInside.Split(";".ToCharArray());
            foreach (String tokenElement in splittedTokens)
            {
                if (versionRegex.IsMatch(tokenElement))
                {
                    Match versionMatcher = versionRegex.Match(tokenElement);
                    GroupCollection groups = versionMatcher.Groups;

                    if (model.confidence > 40)
                    {
                        model.confidence = 95;

                    }
                    else
                    {
                        model.confidence = 85;
                    }

                    if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                    {
                        model.SetDescription(groups[1].Value);
                    }
                    if (groups[2] != null && groups[2].Value.Trim().Length > 0)
                    {
                        model.majorRevision = groups[2].Value;
                    }
                    if (groups[3] != null && groups[3].Value.Trim().Length > 0)
                    {
                        model.minorRevision = groups[3].Value;
                    }
                    if (groups[4] != null && groups[4].Value.Trim().Length > 0)
                    {
                        model.microRevision = groups[4].Value;
                    }
                }

                if (versionMsieRegex.IsMatch(tokenElement))
                {
                    Match versionMsieMatcher = versionMsieRegex.Match(tokenElement);
                    String version = model.GetVersion();
                    if (version == null || version.Length < 7)
                    {
                        version = "0.0.0.0";
                    }
                    String[] subVersion = version.Split(".".ToCharArray());
                    int count = 0;
                    GroupCollection groups = versionMsieMatcher.Groups;
                    for (int idx = 1; idx <= groups.Count; idx++)
                    {
                        if ((idx >= 1) && (idx <= 4) && groups[idx] != null && groups[idx].Value.Trim().Length > 0)
                        {
                            subVersion[idx - 1] = groups[idx].Value;
                            count++;
                        }
                    }
                    model.SetVersion(subVersion[0] + "." + subVersion[1] + "." + subVersion[2] + "." + subVersion[3]);

                    if (model.confidence > 40)
                    {
                        model.confidence = 95;

                    }
                    else
                    {
                        model.confidence = (count * 18);
                    }
                }
            }
            SetWinCeVersion(model);
            return model;
        }

        private void SetWinCeVersion(OSModel.OperatingSystem model)
        {
            //TODO: to be refined
            String version = model.GetVersion();
            if (version == null)
            {
                return;

            }
            else if (!model.majorRevision.Equals("1"))
            {
                return;
            }

            Regex winCEVersionRegex = new Regex(".*(\\d+).(\\d+).(\\d+).(\\d+).*");

            if (winCEVersionRegex.IsMatch(version))
            {
                Match result = winCEVersionRegex.Match(version);
                GroupCollection groups = result.Groups;

                if (groups[1].Value.Equals("4"))
                {
                    model.majorRevision = "5";

                }
                else if (groups[1].Value.Equals("6"))
                {
                    model.majorRevision = "6";

                    if (groups[3].Value.Equals("7"))
                    {
                        model.minorRevision = "1";
                    }
                }
            }
        }
    }
}
