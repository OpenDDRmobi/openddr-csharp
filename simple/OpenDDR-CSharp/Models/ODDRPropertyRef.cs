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
using W3c.Ddr.Simple;

namespace Oddr.Models
{
    public class ODDRPropertyRef : IPropertyRef
    {
        private readonly IPropertyName propertyName;
        private readonly String aspectName;

        public ODDRPropertyRef(IPropertyName propertyName, String aspectName)
        {
            this.propertyName = propertyName;
            this.aspectName = aspectName;
        }

        public string AspectName()
        {
            return aspectName;
        }

        public string LocalPropertyName()
        {
            return propertyName.LocalPropertyName();
        }

        public string Namespace()
        {
            return propertyName.Namespace();
        }

        public string NullAspect()
        {
            return "__NULL";
        }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            ODDRPropertyRef oddr = obj as ODDRPropertyRef;
            if ((System.Object)oddr == null)
            {
                return false;
            }

            return (oddr.AspectName().Equals(this.aspectName) && oddr.LocalPropertyName().Equals(this.LocalPropertyName()) && oddr.Namespace().Equals(this.Namespace()));
        }

        public override int GetHashCode()
        {
            int hash = 3;
            hash = 73 * hash + (this.propertyName != null ? this.propertyName.GetHashCode() : 0);
            hash = 73 * hash + (this.aspectName != null ? this.aspectName.GetHashCode() : 0);
            return hash;
        }
    }
}
