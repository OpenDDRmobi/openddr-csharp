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
using OSModel = Oddr.Models.OS;

namespace Oddr.Builders.OS.Mozilla
{
    public class WinPhoneMozillaSubBuilder : IBuilder
    {
        private const String VERSION_REGEXP = ".*Windows.?Phone.?(?:OS)?.?((\\d+)(?:\\.(\\d+))?(?:\\.(\\d+))?(?:\\.(\\d+))?).*";
        private Regex versionRegex = new Regex(VERSION_REGEXP);

        public bool CanBuild(UserAgent userAgent)
        {
            if (userAgent.containsWindowsPhone)
            {
                Regex windowsPhoneRegex = new Regex(".*Windows.?Phone.*");
                if (windowsPhoneRegex.IsMatch(userAgent.GetPatternElementsInside() + userAgent.GetPatternElementsPost()))
                {
                    return true;
                }
            }
            return false;
        }

        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            OSModel.OperatingSystem model = new OSModel.OperatingSystem();
            model.SetVendor("Microsoft");
            model.SetModel("Windows Phone");
            model.confidence = 40;
            bool isInPostMoz = false;

            String toSplit = userAgent.GetPatternElementsInside();

            if (!versionRegex.IsMatch(toSplit))
            {
                toSplit = userAgent.GetPatternElementsPost();
                isInPostMoz = true;
            }

            String[] splittedTokens = toSplit.Split(";".ToCharArray());
            foreach (String tokenElement in splittedTokens)
            {
                if (versionRegex.IsMatch(tokenElement))
                {
                    Match versionMatcher = versionRegex.Match(tokenElement);
                    GroupCollection groups = versionMatcher.Groups;

                    if (isInPostMoz)
                    {
                        model.confidence = 85;

                    }
                    else
                    {
                        model.confidence = 90;
                    }

                    if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                    {
                        model.SetVersion(groups[1].Value);
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

                    if (groups[5] != null && groups[5].Value.Trim().Length > 0)
                    {
                        model.nanoRevision = groups[5].Value;
                    }
                }

                if (model.majorRevision.Equals("0") && model.minorRevision.Equals("0"))
                {
                    model.majorRevision = "6";
                    model.minorRevision = "5";
                }
                model.SetDescription("Windows Phone " + model.majorRevision + "." + model.minorRevision + "." + model.microRevision + "." + model.nanoRevision);
            }
            return model;
        }
    }
}
