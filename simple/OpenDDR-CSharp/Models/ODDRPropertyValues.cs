﻿/**
 * Copyright 2011 OpenDDR LLC
 * This software is distributed under the terms of the GNU Lesser General Public License.
 *
 *
 * This file is part of OpenDDR Simple APIs.
 * OpenDDR Simple APIs is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, version 3 of the License.
 *
 * OpenDDR Simple APIs is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with Simple APIs.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using W3c.Ddr.Simple;

namespace Oddr.Models
{
    public class ODDRPropertyValues : IPropertyValues
    {
        List<IPropertyValue> properties;

        public ODDRPropertyValues()
        {
            this.properties = new List<IPropertyValue>();
        }

        public void addProperty(IPropertyValue v)
        {
            properties.Add(v);
        }


        public IPropertyValue[] GetAll()
        {
            try
            {
                return properties.ToArray();

            }
            catch (ArgumentNullException ex)
            {
                return new IPropertyValue[0];
            }
        }

        public IPropertyValue GetValue(IPropertyRef pr)
        {
            foreach (IPropertyValue propertyValue in properties)
            {
                if (propertyValue.PropertyRef().Equals(pr))
                {
                    return propertyValue;
                }
            }
            return null;
        }
    }
}
