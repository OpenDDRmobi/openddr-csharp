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
using W3c.Ddr.Simple;

namespace Oddr.Models
{
    /// <summary>
    /// Evidence consisting of HTTP Header name and value pairs.
    /// </summary>
    public class ODDRHTTPEvidence : IEvidence
    {
        /// <summary>
        /// Headers dictionary.
        /// </summary>
        Dictionary<String, String> headers;

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public ODDRHTTPEvidence() {
            headers = new Dictionary<String, String>();
        }

        /// <summary>
        /// Headers Dictionary parameterizable costructor.
        /// </summary>
        /// <param name="map">Headers Dictionary.</param>
        public ODDRHTTPEvidence(Dictionary<String, String> map) {
            headers = new Dictionary<String, String>(map);
        }

        public bool Exist(string property)
        {
            if (property == null)
            {
                return false;
            }
            return headers.ContainsKey(property.ToLower());
        }

        public String Get(String header)
        {
            string toRet = null;
            headers.TryGetValue(header.ToLower(), out toRet);
            return toRet;
        }

        public virtual void Put(String key, String value)
        {
            headers.Add(key.ToLower(), value);
        }
    }
}
