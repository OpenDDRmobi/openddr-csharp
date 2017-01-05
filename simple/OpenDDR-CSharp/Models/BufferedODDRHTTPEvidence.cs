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
using Oddr.Models.Browsers;
using Oddr.Models.Devices;
using System.Runtime.CompilerServices;
using OSModel = Oddr.Models.OS;

namespace Oddr.Models
{
    /// <summary>
    /// Extends ODDRHTTPEvidence. Contains the reference to identified Bowser, Device and OperatingSystem. 
    /// It can be used to retrieve back model object in order to get properties directly.
    /// </summary>
    public class BufferedODDRHTTPEvidence : ODDRHTTPEvidence
    {
        /// <summary>
        /// Identified Browser object. Null if not yet identified.
        /// </summary>
        public Browser browserFound
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        /// <summary>
        /// Identified Device object. Null if not yet identified.
        /// </summary>
        public Device deviceFound
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        /// <summary>
        /// Identified OperatingSystem object. Null if not yet identified.
        /// </summary>
        public OSModel.OperatingSystem osFound
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        /// <summary>
        /// When Evidence change, stored model object are removed in order to allow new identification. 
        /// </summary>
        /// <param name="key">Header name</param>
        /// <param name="value">Header value</param>
        public override void Put(String key, String value) {
            this.osFound = null;
            this.browserFound = null;
            base.Put(key, value);
        }
    }
}
