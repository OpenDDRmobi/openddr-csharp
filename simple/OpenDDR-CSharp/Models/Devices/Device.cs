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

namespace Oddr.Models.Devices
{
    /// <summary>
    /// Identified Device model object.
    /// </summary>
    public class Device : BuiltObject, IComparable, ICloneable
    {
        public String id
        {
            get;
            set;
        }
        public String parentId
        {
            get;
            set;
        }

        public Device()
            : base()
        {
            this.parentId = "root";
        }
        
        public int CompareTo(object obj)
        {
            if (obj == null || !(obj is Device))
                {
                    return int.MaxValue;
                }

            Device bd = (Device) obj;
            return this.confidence - bd.confidence;
        }

        public object Clone()
        {
            Device d = new Device();
            d.id = this.id;
            d.parentId = this.parentId;
            d.confidence = this.confidence;
            d.PutPropertiesMap(this.properties);
            return d;
        }

        public bool ContainsProperty(String propertyName)
        {
            try
            {
                return properties.ContainsKey(propertyName);

            }
            catch (ArgumentNullException ex)
            {
                return false;
            }
        }
    }
}
