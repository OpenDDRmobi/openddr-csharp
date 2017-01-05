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

namespace W3c.Ddr.Simple
{
    /// <summary>
    /// The name of a Property together with its namespace.
    /// </summary>
    public interface IPropertyName
    {
        /// <summary>
        /// Get Property Name
        /// </summary>
        /// <returns>Return the property name</returns>
        String LocalPropertyName();

        /// <summary>
        /// Get Namespace
        /// </summary>
        /// <returns>Return the namespace of the property</returns>
        String Namespace();
    }
}
