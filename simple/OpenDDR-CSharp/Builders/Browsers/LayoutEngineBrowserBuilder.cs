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
    public abstract class LayoutEngineBrowserBuilder : HintedResolutionBrowserBuilder
    {
        protected const String APPLEWEBKIT = "AppleWebKit";
        protected const String PRESTO = "Presto";
        protected const String GECKO = "Gecko";
        protected const String TRIDENT = "Trident";
        protected const String KHML = "KHTML";
        private const String WEBKIT_VERSION_REGEXP = ".*AppleWebKit/([0-9\\.]+).*?";
        private const String PRESTO_VERSION_REGEXP = ".*Presto/([0-9\\.]+).*?";
        private const String GECKO_VERSION_REGEXP = ".*Gecko/([0-9\\.]+).*?";
        private const String TRIDENT_VERSION_REGEXP = ".*Trident/([0-9\\.]+).*?";
        private const String KHTML_VERSION_REGEXP = ".*KHTML/([0-9\\.]+).*?";
        private static Regex webkitVersionRegex = new Regex(WEBKIT_VERSION_REGEXP, RegexOptions.Compiled);
        private static Regex prestoVersionRegex = new Regex(PRESTO_VERSION_REGEXP, RegexOptions.Compiled);
        private static Regex geckoVersionRegex = new Regex(GECKO_VERSION_REGEXP, RegexOptions.Compiled);
        private static Regex tridentVersionRegex = new Regex(TRIDENT_VERSION_REGEXP, RegexOptions.Compiled);
        private static Regex khtmlVersionRegex = new Regex(KHTML_VERSION_REGEXP, RegexOptions.Compiled);


        protected override Browser BuildBrowser(UserAgent userAgent, int hintedWidth, int hintedHeight)
        {
            String layoutEngine = null;
            String layoutEngineVersion = null;

            Match match = null;

            if (webkitVersionRegex.IsMatch(userAgent.completeUserAgent))
            {
                match = webkitVersionRegex.Match(userAgent.completeUserAgent);
                layoutEngine = APPLEWEBKIT;
                GroupCollection groups = match.Groups;
                layoutEngineVersion = groups[1].Value;

            }
            else
            {

                if (prestoVersionRegex.IsMatch(userAgent.completeUserAgent))
                {
                    match = prestoVersionRegex.Match(userAgent.completeUserAgent);
                    layoutEngine = "Presto";
                    GroupCollection groups = match.Groups;
                    layoutEngineVersion = groups[1].Value;

                }
                else
                {
                    if (geckoVersionRegex.IsMatch(userAgent.completeUserAgent))
                    {
                        match = geckoVersionRegex.Match(userAgent.completeUserAgent);
                        layoutEngine = "Gecko";
                        GroupCollection groups = match.Groups;
                        layoutEngineVersion = groups[1].Value;

                    }
                    else
                    {
                        if (tridentVersionRegex.IsMatch(userAgent.completeUserAgent))
                        {
                            match = tridentVersionRegex.Match(userAgent.completeUserAgent);
                            layoutEngine = "Trident";
                            GroupCollection groups = match.Groups;
                            layoutEngineVersion = groups[1].Value;

                        }
                        else
                        {
                            if (khtmlVersionRegex.IsMatch(userAgent.completeUserAgent))
                            {
                                match = khtmlVersionRegex.Match(userAgent.completeUserAgent);
                                layoutEngine = "KHTML";
                                GroupCollection groups = match.Groups;
                                layoutEngineVersion = groups[1].Value;
                            }
                        }
                    }
                }
            }
            return BuildBrowser(userAgent, layoutEngine, layoutEngineVersion, hintedWidth, hintedHeight);
        }

        protected abstract Browser BuildBrowser(UserAgent userAgent, String layoutEngine, String layoutEngineVersion, int hintedWidth, int hintedHeight);
    }
}
