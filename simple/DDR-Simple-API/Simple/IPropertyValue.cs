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
    /// PropertyValue models a PropertyRef together with its value
    /// </summary>
    /// <remarks>
    /// Values may be empty, in which case the method exists returns false. An attempt to query an empty value causes a ValueException as does an attempt to query a value with an incompatible accessor method (string as float, for example). For the getString method implementations must return an implementation dependent String representation if the type of the value is not natively String. For other methods if the underlying type of the data does not match the method signature then a ValueException must be thrown.
    /// </remarks>
    public interface IPropertyValue
    {
        /// <summary>Value Retrieval</summary>
        /// <exception cref="ValueException">Throws when query an empty value or attempt to query a value with an incompatible type</exception>
        double GetDouble();

        /// <summary>Value Retrieval</summary>
        /// <exception cref="ValueException">Throws when query an empty value or attempt to query a value with an incompatible type</exception>
        long GetLong();

        /// <summary>Value Retrieval</summary>
        /// <exception cref="ValueException">Throws when query an empty value or attempt to query a value with an incompatible type</exception>
        bool GetBoolean();

        /// <summary>Value Retrieval</summary>
        /// <exception cref="ValueException">Throws when query an empty value or attempt to query a value with an incompatible type</exception>
        int GetInteger();

        /// <summary>Value Retrieval</summary>
        /// <exception cref="ValueException">Throws when query an empty value or attempt to query a value with an incompatible type</exception>
        String[] GetEnumeration();

        /// <summary>Value Retrieval</summary>
        /// <exception cref="ValueException">Throws when query an empty value or attempt to query a value with an incompatible type</exception>
        float GetFloat();

        /// <summary>
        /// Property Reference
        /// </summary>
        /// <returns>The PropertyRef that this PropertyValue refers to.</returns>
        IPropertyRef PropertyRef();

        /// <summary>Value Retrieval</summary>
        /// <exception cref="ValueException">Throws when query an empty value or attempt to query a value with an incompatible type</exception>
        String GetString();

        /// <summary>
        /// Existence
        /// </summary>
        /// <returns>True if a value is available, false otherwise.</returns>
        bool Exists();
    }
}
