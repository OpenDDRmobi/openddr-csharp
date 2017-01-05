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
using W3c.Ddr.Exceptions;

namespace Oddr.Models
{
    public class ODDRPropertyValue : IPropertyValue
    {
        private const String TYPE_BOOLEAN = "xs:boolean";
        private const String TYPE_DOUBLE = "xs:double";
        private const String TYPE_ENUMERATION = "xs:enumeration";
        private const String TYPE_FLOAT = "xs:float";
        private const String TYPE_INT = "xs:integer";
        private const String TYPE_NON_NEGATIVE_INTEGER = "xs:nonNegativeInteger";
        private const String TYPE_LONG = "xs:long";
        private readonly String value;
        private readonly String type;
        private readonly IPropertyRef propertyRef;

        public ODDRPropertyValue(String value, String type, IPropertyRef propertyRef)
        {
            this.value = value == null ? value : value.Trim();
            this.type = type;
            this.propertyRef = propertyRef;
        }

        public bool Exists()
        {
            if (value != null && value.Length > 0 && !"-".Equals(value))
            {
                return true;
            }
            return false;
        }

        /// <exception cref="W3c.Ddr.Exceptions.ValueException">Thrown when...</exception>
        public bool GetBoolean()
        {
            if (!Exists())
            {
                throw new ValueException(ValueException.NOT_KNOWN, type);
            }
            if (type.Equals(TYPE_BOOLEAN))
            {
                try
                {
                    return bool.Parse(value);

                }
                catch (FormatException ex)
                {
                    throw new ValueException(ValueException.INCOMPATIBLE_TYPES, ex);
                }
            }
            throw new ValueException(ValueException.INCOMPATIBLE_TYPES, "Not " + TYPE_BOOLEAN + " value");
        }

        /// <exception cref="W3c.Ddr.Exceptions.ValueException">Thrown when...</exception>
        public double GetDouble()
        {
            if (!Exists())
            {
                throw new ValueException(ValueException.NOT_KNOWN, type);
            }

            if (type.Equals(TYPE_DOUBLE) || type.Equals(TYPE_FLOAT))
            {
                try
                {
                    return double.Parse(value);

                }
                catch (FormatException ex)
                {
                    throw new ValueException(ValueException.INCOMPATIBLE_TYPES, ex);
                }
            }
            throw new ValueException(ValueException.INCOMPATIBLE_TYPES, "Not " + TYPE_DOUBLE + " value");
        }

        /// <exception cref="W3c.Ddr.Exceptions.ValueException">Thrown when...</exception>
        public string[] GetEnumeration()
        {
            if (!Exists())
            {
                throw new ValueException(ValueException.NOT_KNOWN, type);
            }

            if (type.Equals(TYPE_ENUMERATION))
            {
                try
                {
                    String[] splitted = value.Split(",".ToCharArray());
                    for (int i = 0; i < splitted.Length; i++)
                    {
                        splitted[i] = splitted[i].Trim();
                    }

                    return splitted;

                }
                catch (Exception ex)
                {
                    throw new ValueException(ValueException.INCOMPATIBLE_TYPES, ex);
                }
            }
            throw new ValueException(ValueException.INCOMPATIBLE_TYPES, "Not " + TYPE_ENUMERATION + " value");
        }

        /// <exception cref="W3c.Ddr.Exceptions.ValueException">Thrown when...</exception>
        public float GetFloat()
        {
            if (!Exists())
            {
                throw new ValueException(ValueException.NOT_KNOWN, type);
            }

            if (type.Equals(TYPE_FLOAT))
            {
                try
                {
                    return float.Parse(value);

                }
                catch (FormatException ex)
                {
                    throw new ValueException(ValueException.INCOMPATIBLE_TYPES, ex);
                }
            }
            throw new ValueException(ValueException.INCOMPATIBLE_TYPES, "Not " + TYPE_FLOAT + " value");
        }

        /// <exception cref="W3c.Ddr.Exceptions.ValueException">Thrown when...</exception>
        public int GetInteger()
        {
            if (!Exists())
            {
                throw new ValueException(ValueException.NOT_KNOWN, type);
            }

            if (type.Equals(TYPE_INT))
            {
                try
                {
                    return int.Parse(value);

                }
                catch (FormatException ex)
                {
                    throw new ValueException(ValueException.INCOMPATIBLE_TYPES, ex);
                }
            }

            if (type.Equals(TYPE_NON_NEGATIVE_INTEGER))
            {
                try
                {
                    int integer = int.Parse(value);

                    if (integer >= 0)
                    {
                        return integer;
                    }

                }
                catch (FormatException ex)
                {
                    throw new ValueException(ValueException.INCOMPATIBLE_TYPES, ex);
                }
            }
            throw new ValueException(ValueException.INCOMPATIBLE_TYPES, "Not " + TYPE_INT + " value");
        }

        /// <exception cref="W3c.Ddr.Exceptions.ValueException">Thrown when...</exception>
        public long GetLong()
        {
            if (!Exists())
            {
                throw new ValueException(ValueException.NOT_KNOWN, type);
            }
            if (type.Equals(TYPE_LONG) || type.Equals(TYPE_INT) || type.Equals(TYPE_NON_NEGATIVE_INTEGER))
            {
                try
                {
                    return long.Parse(value);

                }
                catch (FormatException ex)
                {
                    throw new ValueException(ValueException.INCOMPATIBLE_TYPES, ex);
                }
            }
            throw new ValueException(ValueException.INCOMPATIBLE_TYPES, "Not " + TYPE_LONG + " value");
        }

        /// <exception cref="W3c.Ddr.Exceptions.ValueException">Thrown when...</exception>
        public string GetString()
        {
            if (!Exists())
            {
                throw new ValueException(ValueException.NOT_KNOWN, type);
            }
            return value;
        }

        public IPropertyRef PropertyRef()
        {
            return this.propertyRef;
        }
    }
}
