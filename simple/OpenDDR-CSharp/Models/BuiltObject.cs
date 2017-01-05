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

namespace Oddr.Models
{
    /// <summary>
    /// Superclass of identified model object.
    /// </summary>
    public class BuiltObject
    {
        /// <summary>
        /// Confidence of identified model object.
        /// </summary>
        public int confidence
        {
            set;
            get;
        }
        /// <summary>
        /// Dictionary of properties of identified model object.
        /// </summary>
        public Dictionary<String, String> properties
        {
            protected set;
            get;
        }

        public BuiltObject() {
            this.properties = new Dictionary<String, String>();
            this.confidence = 0;
        }

        public BuiltObject(int confidence, Dictionary<String, String> properties)
        {
            this.confidence = confidence;
            this.properties = properties;
        }

        public BuiltObject(Dictionary<String, String> properties)
        {
            this.confidence = 0;
            this.properties = properties;
        }

        /// <summary>
        /// Retrieve a property from properties dictionary.
        /// </summary>
        /// <param name="property">The name of requested properties.</param>
        /// <returns>Return the value of requested property.</returns>
        public String Get(String property)
        {
            if (properties.ContainsKey(property))
            {
                return properties[property];
            }
            return null;
        }

        /// <summary>
        /// Add a property to properties dictionary.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        public void PutProperty(String name, String value) {
            this.properties[name] = value;
        }

        public void PutPropertiesMap(Dictionary<String, String> properties)
        {
            foreach (KeyValuePair<string, string> kvp in properties)
            {
                this.properties[kvp.Key] = kvp.Value;
            }
        }
    }
}
