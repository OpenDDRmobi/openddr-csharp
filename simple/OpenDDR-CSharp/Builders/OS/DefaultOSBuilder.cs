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

namespace Oddr.Builders.OS
{
    public class DefaultOSBuilder : IBuilder
    {
        private IBuilder[] builders;
        private static volatile DefaultOSBuilder instance;
        private static object syncRoot = new Object();

        private DefaultOSBuilder()
        {
            builders = new IBuilder[]
            {
                new MozillaOSModelBuilder(),
                new OperaOSModelBuilder(),
                new BlackBerryOSBuilder()
            };
        }

        public static DefaultOSBuilder Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new DefaultOSBuilder();
                        }
                    }
                }

                return instance;
            }
        }

        public bool CanBuild(UserAgent userAgent)
        {
            foreach (IBuilder builder in builders)
            {
                if (builder.CanBuild(userAgent))
                {
                    return true;
                }
            }
            return false;
        }

        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            List<OSModel.OperatingSystem> founds = new List<OSModel.OperatingSystem>();
            OSModel.OperatingSystem found = null;
            foreach (IBuilder builder in builders)
            {
                if (builder.CanBuild(userAgent))
                {
                    OSModel.OperatingSystem builded = (OSModel.OperatingSystem)builder.Build(userAgent, confidenceTreshold);
                    if (builded != null)
                    {
                        founds.Add(builded);
                        if (builded.confidence >= confidenceTreshold)
                        {
                            found = builded;
                            break;
                        }
                    }
                }
            }

            if (found != null)
            {
                return found;
            }
            else
            {
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
