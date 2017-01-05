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
using OSModel = Oddr.Models.OS;
using Oddr.Builders.OS.Mozilla;

namespace Oddr.Builders.OS
{
    public class MozillaOSModelBuilder : IBuilder
    {
        private IBuilder[] builders;

        public MozillaOSModelBuilder()
        {
            builders = new IBuilder[]
            {
                new IOSMozillaSubBuilder(),
                new AndroidMozillaSubBuilder(),
                new WinPhoneMozillaSubBuilder(),
                new BlackBerryMozillaSubBuilder(),
                new SymbianMozillaSubBuilder(),
                new WinCEMozillaSubBuilder(),
                new BadaMozillaSubBuilder(),
                new BrewMozillaSubBuilder(),
                new WebOSMozillaSubBuilder(),
                new LinuxMozillaSubBuilder(),
                new MacOSXMozillaSubBuilder()
            };
        }

        public bool CanBuild(UserAgent userAgent)
        {
            return userAgent.mozillaPattern;
        }

        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            List<OSModel.OperatingSystem> founds = new List<OSModel.OperatingSystem>();
            OSModel.OperatingSystem found = null;
            foreach (IBuilder builder in builders) {
                if (builder.CanBuild(userAgent)) {
                    OSModel.OperatingSystem builded = (OSModel.OperatingSystem) builder.Build(userAgent, confidenceTreshold);
                    if (builded != null) {
                        founds.Add(builded);
                        if (builded.confidence >= confidenceTreshold) {
                            found = builded;
                            break;
                        }
                    }
                }
            }

            if (found != null) {
                return found;

            } else {
                if (founds.Count > 0)
                {
                    founds.Sort();
                    founds.Reverse();
                    return founds[0];
                }

                return null;
            }
        }
    }
}
