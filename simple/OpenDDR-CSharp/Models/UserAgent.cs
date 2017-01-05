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

namespace Oddr.Models
{
    public class UserAgent
    {

        public const String MOZILLA_AND_OPERA_PATTERN = "(.*?)((?:Mozilla)|(?:Opera))[/ ](\\d+\\.\\d+).*?\\(((?:.*?)(?:.*?\\(.*?\\))*(?:.*?))\\)(.*)";
        private const String VERSION_PATTERN = ".*Version/(\\d+.\\d+).*";
        public const int INDEX_MOZILLA_PATTERN_GROUP_PRE = 1;
        public const int INDEX_MOZILLA_PATTERN_GROUP_INSIDE = 4;
        public const int INDEX_MOZILLA_PATTERN_GROUP_POST = 5;
        public const int INDEX_MOZILLA_PATTERN_GROUP_MOZ_VER = 3;
        public const int INDEX_OPERA_OR_MOZILLA = 2;
        private static Regex mozillaPatternCompiled = new Regex(MOZILLA_AND_OPERA_PATTERN, RegexOptions.Compiled);
        private static Regex versionPatternCompiled = new Regex(VERSION_PATTERN, RegexOptions.Compiled);
        private static Regex iPadRegex = new Regex(".*(?!like).iPad.*", RegexOptions.Compiled);
        private static Regex iPodRegex = new Regex(".*(?!like).iPod.*", RegexOptions.Compiled);
        private static Regex iPhoneRegex = new Regex(".*(?!like).iPhone.*", RegexOptions.Compiled);
        private static Regex blackBerryRegex = new Regex(".*[Bb]lack.?[Bb]erry.*|.*RIM.?Tablet.?OS.*", RegexOptions.Compiled);
        private static Regex symbianRegex = new Regex(".*Symbian.*|.*SymbOS.*|.*Series.?60.*", RegexOptions.Compiled);
        private static Regex windowsRegex = new Regex(".*Windows.?(?:(?:CE)|(?:Phone)|(?:NT)|(?:Mobile)).*", RegexOptions.Compiled);
        private static Regex internetExplorerRegex = new Regex(".*MSIE.([0-9\\.b]+).*", RegexOptions.Compiled);
        public String completeUserAgent
        {
            private set;
            get;
        }

        public bool mozillaPattern
        {
            private set;
            get;
        }

        public bool operaPattern
        {
            private set;
            get;
        }

        public String mozillaVersion
        {
            private set;
            get;
        }

        public String operaVersion
        {
            private set;
            get;
        }

        public bool containsAndroid
        {
            private set;
            get;
        }

        public bool containsBlackBerryOrRim
        {
            private set;
            get;
        }

        public bool containsIOSDevices
        {
            private set;
            get;
        }

        public bool containsMSIE
        {
            private set;
            get;
        }

        public bool containsSymbian
        {
            private set;
            get;
        }

        public bool containsWindowsPhone
        {
            private set;
            get;
        }

        private String[] patternElements;

        internal UserAgent(String userAgent)
        {
            if (userAgent == null)
            {
                throw new ArgumentNullException("userAgent can not be null");
            }
            completeUserAgent = userAgent;

            Match result = mozillaPatternCompiled.Match(userAgent);

            if (result.Success)
            {
                patternElements = new String[]{
                        result.Groups[INDEX_MOZILLA_PATTERN_GROUP_PRE].Value,
                        result.Groups[INDEX_MOZILLA_PATTERN_GROUP_INSIDE].Value,
                        result.Groups[INDEX_MOZILLA_PATTERN_GROUP_POST].Value
                    };
                String version = result.Groups[INDEX_MOZILLA_PATTERN_GROUP_MOZ_VER].Value;
                if (result.Groups[INDEX_OPERA_OR_MOZILLA].Value.Contains("Opera"))
                {
                    mozillaPattern = false;
                    operaPattern = true;
                    operaVersion = version;

                    if (operaVersion.Equals("9.80") && patternElements[2] != null)
                    {
                        Match result2 = versionPatternCompiled.Match(patternElements[2]);

                        if (result2.Success)
                        {
                            operaVersion = result2.Groups[1].Value;
                        }
                    }

                }
                else
                {
                    mozillaPattern = true;
                    mozillaVersion = version;
                }

            }
            else
            {
                mozillaPattern = false;
                operaPattern = false;
                patternElements = new String[]{
                        null,
                        null,
                        null};
                mozillaVersion = null;
                operaVersion = null;
            }

            if (userAgent.Contains("Android"))
            {
                containsAndroid = true;

            }
            else
            {
                containsAndroid = false;

                if (iPadRegex.IsMatch(userAgent) || iPodRegex.IsMatch(userAgent) || iPhoneRegex.IsMatch(userAgent))
                {
                    containsIOSDevices = true;

                }
                else
                {
                    containsIOSDevices = false;
                    if (blackBerryRegex.IsMatch(userAgent))
                    {
                        containsBlackBerryOrRim = true;

                    }
                    else
                    {
                        containsBlackBerryOrRim = false;
                        if (symbianRegex.IsMatch(userAgent))
                        {
                            containsSymbian = true;

                        }
                        else
                        {
                            containsSymbian = false;
                            if (windowsRegex.IsMatch(userAgent))
                            {
                                containsWindowsPhone = true;

                            }
                            else
                            {
                                containsWindowsPhone = false;
                            }

                            if (internetExplorerRegex.IsMatch(userAgent))
                            {
                                containsMSIE = true;

                            }
                            else
                            {
                                containsMSIE = false;
                            }
                        }
                    }
                }
            }
        }

        public String GetPatternElementsPre()
        {
            return patternElements[0];
        }

        public String GetPatternElementsInside()
        {
            return patternElements[1];
        }

        public String GetPatternElementsPost()
        {
            return patternElements[2];
        }
    }
}
