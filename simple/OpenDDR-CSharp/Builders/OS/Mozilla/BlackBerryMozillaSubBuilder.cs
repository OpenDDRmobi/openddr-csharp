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
    public class BlackBerryMozillaSubBuilder : IBuilder
    {
        private const String VERSION_REGEXP = "(?:.*?Version.?((\\d+)\\.(\\d+)(?:\\.(\\d+))?(?:\\.(\\d+))?).*)|(?:.*?[Bb]lack.?[Bb]erry(?:\\d+)/((\\d+)\\.(\\d+)(?:\\.(\\d+))?(?:\\.(\\d+))?).*)|(?:.*?RIM.?Tablet.?OS.?((\\d+)\\.(\\d+)(?:\\.(\\d+))?(?:\\.(\\d+))?).*)";
        private Regex versionRegex = new Regex(VERSION_REGEXP);

        public bool CanBuild(UserAgent userAgent)
        {
            return userAgent.containsBlackBerryOrRim;
        }

        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            OSModel.OperatingSystem model = new OSModel.OperatingSystem();

            String rebuilded = userAgent.GetPatternElementsInside() + ";" + userAgent.GetPatternElementsPost();

            String[] splittedTokens = rebuilded.Split(";".ToCharArray());
            foreach (String tokenElement in splittedTokens)
            {
                if (versionRegex.IsMatch(tokenElement))
                {
                    Match versionMatcher = versionRegex.Match(tokenElement);
                    GroupCollection groups = versionMatcher.Groups;

                    if (groups[11].Value != null && groups[11].Value.Trim().Length > 0)
                    {
                        model.SetVendor("Research In Motion");
                        model.SetModel("RIM Tablet OS");
                        model.majorRevision = "1";
                        model.confidence = 50;

                        if (groups[11].Value != null && groups[11].Value.Trim().Length > 0)
                        {
                            model.SetVersion(groups[11].Value);

                        }

                        if (groups[12].Value != null && groups[12].Value.Trim().Length > 0)
                        {
                            model.majorRevision = groups[12].Value;
                            model.confidence = 60;

                        }

                        if (groups[13].Value != null && groups[13].Value.Trim().Length > 0)
                        {
                            model.minorRevision = groups[13].Value;
                            model.confidence = 70;

                        }

                        if (groups[14].Value != null && groups[14].Value.Trim().Length > 0)
                        {
                            model.microRevision = groups[14].Value;
                            model.confidence = 80;

                        }

                        if (groups[15].Value != null && groups[15].Value.Trim().Length > 0)
                        {
                            model.nanoRevision = groups[15].Value;
                            model.confidence = 90;

                        }
                        return model;

                    }
                    else if ((groups[1] != null && groups[1].Value.Trim().Length > 0) || (groups[6] != null && groups[6].Value.Trim().Length > 0))
                    {
                        model.SetVendor("Research In Motion");
                        model.SetModel("Black Berry OS");
                        model.majorRevision = "1";
                        model.confidence = 40;

                        if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                        {
                            if (groups[6] != null && groups[6].Value.Trim().Length > 0)
                            {
                                model.confidence = 100;

                            }
                            else
                            {
                                model.confidence = 80;
                            }

                        }
                        else if (groups[6] != null && groups[6].Value.Trim().Length > 0)
                        {
                            model.confidence = 90;
                        }

                        if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                        {
                            model.SetVersion(groups[1].Value);

                        }
                        else if (groups[6] != null && groups[6].Value.Trim().Length > 0)
                        {
                            model.SetVersion(groups[6].Value);
                        }

                        if (groups[2] != null && groups[2].Value.Trim().Length > 0)
                        {
                            model.majorRevision = groups[2].Value;

                        }
                        else if (groups[7] != null && groups[7].Value.Trim().Length > 0)
                        {
                            model.majorRevision = groups[7].Value;
                        }

                        if (groups[3] != null && groups[3].Value.Trim().Length > 0)
                        {
                            model.minorRevision = groups[3].Value;

                        }
                        else if (groups[8] != null && groups[8].Value.Trim().Length > 0)
                        {
                            model.minorRevision = groups[8].Value;
                        }

                        if (groups[4] != null && groups[4].Value.Trim().Length > 0)
                        {
                            model.microRevision = groups[4].Value;

                        }
                        else if (groups[9] != null && groups[9].Value.Trim().Length > 0)
                        {
                            model.microRevision = groups[9].Value;
                        }

                        if (groups[5] != null && groups[5].Value.Trim().Length > 0)
                        {
                            model.nanoRevision = groups[5].Value;

                        }
                        else if (groups[10] != null && groups[10].Value.Trim().Length > 0)
                        {
                            model.nanoRevision = groups[10].Value;
                        }
                        return model;

                    }
                }
            }
            return model;
        }
    }
}
