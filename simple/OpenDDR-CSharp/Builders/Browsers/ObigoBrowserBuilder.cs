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
    public class ObigoBrowserBuilder : LayoutEngineBrowserBuilder
    {
        private const String VERSION_REGEXP = ".*?(?:(?:ObigoInternetBrowser/)|(?:Obigo Browser )|(?:[Oo]bigo[- ][Bb]rowser/))([0-9A-Z\\.]+).*?";
        private const String VERSION_REGEXP2 = ".*?(?:(?:Browser/Obigo)|(?:OBIGO[/_-])|(?:Obigo[-/ ]))([0-9A-Z\\.]+).*?";
        private const String VERSION_REGEXP3 = ".*?(?:(?:Obigo[Il]nternetBrowser/)|(?:Obigo Browser )|(?:[Oo]bigo[- ][Bb]rowser/))([0-9A-Zacqv\\.]+).*?";
        private const String VERSION_REGEXP4 = ".*?(?:(?:[Bb]rowser/[Oo]bigo)|(?:OBIGO[/_-])|(?:Obigo[-/ ]))([0-9A-Zacqv\\.]+).*?";
        private const String VERSION_REGEXP5 = ".*?(?:(?:[Tt]eleca Q))([0-9A-Zacqv\\.]+).*?";
        private static Regex versionRegex = new Regex(VERSION_REGEXP, RegexOptions.Compiled);
        private static Regex versionRegex2 = new Regex(VERSION_REGEXP2, RegexOptions.Compiled);
        private static Regex versionRegex3 = new Regex(VERSION_REGEXP3, RegexOptions.Compiled);
        private static Regex versionRegex4 = new Regex(VERSION_REGEXP4, RegexOptions.Compiled);
        private static Regex versionRegex5 = new Regex(VERSION_REGEXP5, RegexOptions.Compiled);

        private const string OBIGO_TELECA_REGEXP = /*"(?i)(.*obigo.*)|(.*teleca.*)"*/"(.*obigo.*)|(.*teleca.*)";
        private static Regex obigoTelecaRegex = new Regex(OBIGO_TELECA_REGEXP, RegexOptions.Compiled | RegexOptions.IgnoreCase);



        protected override Browser BuildBrowser(UserAgent userAgent, string layoutEngine, string layoutEngineVersion, int hintedWidth, int hintedHeight)
        {
            String version = null;

            int confidence = 60;
            Browser identified = new Browser();
            identified.SetVendor("Obigo");
            identified.SetModel("Obigo Browser");

            if (!versionRegex.IsMatch(userAgent.completeUserAgent))
            {
                version = null;

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

            if (version == null)
            {
                if (!versionRegex2.IsMatch(userAgent.completeUserAgent))
                {
                    version = null;

                }
                else
                {
                    Match versionMatcher2 = versionRegex2.Match(userAgent.completeUserAgent);
                    GroupCollection groups = versionMatcher2.Groups;

                    if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                    {
                        version = groups[1].Value;
                    }
                }
            }

            if (version == null)
            {
                if (!versionRegex3.IsMatch(userAgent.completeUserAgent))
                {
                    version = null;

                }
                else
                {
                    Match versionMatcher3 = versionRegex3.Match(userAgent.completeUserAgent);
                    GroupCollection groups = versionMatcher3.Groups;

                    if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                    {
                        version = groups[1].Value;
                    }
                }
            }

            if (version == null)
            {
                if (!versionRegex4.IsMatch(userAgent.completeUserAgent))
                {
                    version = null;

                }
                else
                {
                    Match versionMatcher4 = versionRegex4.Match(userAgent.completeUserAgent);
                    GroupCollection groups = versionMatcher4.Groups;

                    if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                    {
                        version = groups[1].Value;
                    }
                }
            }

            if (version == null)
            {
                if (!versionRegex5.IsMatch(userAgent.completeUserAgent))
                {
                    version = null;

                }
                else
                {
                    Match versionMatcher5 = versionRegex5.Match(userAgent.completeUserAgent);
                    GroupCollection groups = versionMatcher5.Groups;

                    if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                    {
                        version = groups[1].Value;
                        identified.SetModel("Teleca-Obigo");
                    }
                }
            }

            if (version == null)
            {
                return null;
            }

            identified.SetVersion(version);

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
            if (obigoTelecaRegex.IsMatch(userAgent.completeUserAgent))
            {
                return true;
            }
            return false;
        }
    }
}
