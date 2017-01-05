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
    /// Evidence is the general term applied to providing information to the DDR to allow it to determine the Delivery Context.
    /// </summary>
    /// <remarks>
    /// In the DDR Simple API implementations must support Evidence consisting of HTTP Header name and value pairs. Implementations must treat HTTP Header names in a case insensitive manner. HTTP Header values may be case sensitive, depending on the header concerned. Other types of Evidence may be supported by implementations. They are not defined in http://www.w3.org/TR/DDR-Simple-API/ Recommendation.
    /// </remarks>
    public interface IEvidence
    {
        /// <summary>
        /// Add Evidence
        /// </summary>
        /// <param name="key">Header name</param>
        /// <param name="value">Header value</param>
        void Put(String key, String value);

        /// <summary>
        /// Query Evidence
        /// </summary>
        /// <param name="key">Header name</param>
        /// <returns>Return true if requested evidence exist</returns>
        bool Exist(String key);

        /// <summary>
        /// Retrieve Evidence
        /// </summary>
        /// <param name="key">Header name</param>
        /// <returns>Return the value of the Evidence identified by key</returns>
        String Get(String key);
    }
}
