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
using OSModel = Oddr.Models.OS;
using Oddr.Models;

namespace Oddr.Builders.OS.Mozilla
{
    public class SymbianMozillaSubBuilder : IBuilder
    {
        private const String VERSION_REGEXP = ".*Series.?60/(\\d+)(?:[\\.\\- ](\\d+))?(?:[\\.\\- ](\\d+))?.*";
        private const String VERSION_EXTRA = ".*Symbian(?:OS)?/(.*)";
        private Regex versionRegex = new Regex(VERSION_REGEXP);
        private Regex versionExtraRegex = new Regex(VERSION_EXTRA);

        public bool CanBuild(UserAgent userAgent)
        {
            return userAgent.containsSymbian;
        }

        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            OSModel.OperatingSystem model = new OSModel.OperatingSystem();
            model.majorRevision = "1";
            model.SetVendor("Nokia");
            model.SetModel("Symbian OS");
            model.confidence = 40;

            string patternElementsInside = userAgent.GetPatternElementsInside();
            String[] splittedTokens = patternElementsInside.Split(";".ToCharArray());
            foreach (String tokenElement in splittedTokens)
            {
                if (versionRegex.IsMatch(tokenElement))
                {
                    Match versionMatcher = versionRegex.Match(tokenElement);
                    GroupCollection groups = versionMatcher.Groups;

                    model.SetDescription("Series60");
                    if (model.confidence > 40)
                    {
                        model.confidence = 100;

                    }
                    else
                    {
                        model.confidence = 90;
                    }

                    if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                    {
                        model.majorRevision = groups[1].Value;
                    }
                    if (groups[2] != null && groups[2].Value.Trim().Length > 0)
                    {
                        model.minorRevision = groups[2].Value;
                    }
                    if (groups[3] != null && groups[3].Value.Trim().Length > 0)
                    {
                        model.microRevision = groups[3].Value;
                    }
                }

                if (versionExtraRegex.IsMatch(tokenElement))
                {
                    Match versionExtraMatcher = versionExtraRegex.Match(tokenElement);
                    GroupCollection groups = versionExtraMatcher.Groups;

                    if (model.confidence > 40)
                    {
                        model.confidence = 100;

                    }
                    else
                    {
                        model.confidence = 85;
                    }

                    if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                    {
                        string groupValueTrimmed = groups[1].Value.Trim();
                        model.SetVersion(groupValueTrimmed);
                    }
                }
                //TODO: inference VERSION_EXTRA/VERSION_REGEXP and vice-versa
            }
            return model;
        }
    }
}
