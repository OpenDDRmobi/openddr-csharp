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
    public abstract class HintedResolutionBrowserBuilder : IBuilder
    {
        private const String RESOLUTION_HINT_WXH_REGEXP = ".*([0-9][0-9][0-9]+)[*Xx]([0-9][0-9][0-9]+).*";
        private const String RESOLUTION_HINT_FWVGA_REGEXP = ".*FWVGA.*";
        private const String RESOLUTION_HINT_WVGA_REGEXP = ".*WVGA.*";
        private const String RESOLUTION_HINT_WXGA_REGEXP = ".*WXGA.*";
        private const String RESOLUTION_HINT_WQVGA_REGEXP = ".*WQVGA.*";
        private static Regex resolutionHintWxHRegex = new Regex(RESOLUTION_HINT_WXH_REGEXP, RegexOptions.Compiled);
        private static Regex resolutionHintFWVGARegex = new Regex(RESOLUTION_HINT_FWVGA_REGEXP, RegexOptions.Compiled);
        private static Regex resolutionHintWVGARegex = new Regex(RESOLUTION_HINT_WVGA_REGEXP, RegexOptions.Compiled);
        private static Regex resolutionHintWXGARegex = new Regex(RESOLUTION_HINT_WXGA_REGEXP, RegexOptions.Compiled);
        private static Regex resolutionHintWQVGARegex = new Regex(RESOLUTION_HINT_WQVGA_REGEXP, RegexOptions.Compiled);


        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            int hintedWidth = -1;
            int hintedHeight = -1;

            if (resolutionHintWxHRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match match = resolutionHintWxHRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = match.Groups;
                int.TryParse(groups[0].Value, out hintedWidth);
                int.TryParse(groups[1].Value, out hintedHeight);
            }
            else if (userAgent.completeUserAgent.Contains("VGA") || userAgent.completeUserAgent.Contains("WXGA"))
            {
                if (resolutionHintFWVGARegex.IsMatch(userAgent.completeUserAgent))
                {
                    hintedWidth = 480;
                    hintedHeight = 854;
                }
                else if (resolutionHintWVGARegex.IsMatch(userAgent.completeUserAgent))
                {
                    hintedWidth = 480;
                    hintedHeight = 800;
                }
                else if (resolutionHintWXGARegex.IsMatch(userAgent.completeUserAgent))
                {
                    hintedWidth = 768;
                    hintedHeight = 1280;
                }
                else if (resolutionHintWQVGARegex.IsMatch(userAgent.completeUserAgent))
                {
                    hintedWidth = 240;
                    hintedHeight = 400;
                }
            }

            return BuildBrowser(userAgent, hintedWidth, hintedHeight);
        }

        protected abstract Browser BuildBrowser(UserAgent userAgent, int hintedWidth, int hintedHeight);

        public abstract bool CanBuild(UserAgent userAgent);
    }
}
