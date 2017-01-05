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

namespace W3c.Ddr.Exceptions
{
    /// <summary>
    /// This is a subclass of DDRException and is thrown when an error is detected during an attempt to retrieve the value of a Property using one of the value accessor methods of the PropertyValue class. The exception code indicates the nature of the error.
    /// </summary>
    public class ValueException : DDRException
    {
        /// <summary>
        /// The value represented by the PropertyValue is incompatible with the return type of the method used to retrieve it.
        /// </summary>
        public static int INCOMPATIBLE_TYPES = 600;

        /// <summary>
        /// The property value is unknown.
        /// </summary>
        public static int NOT_KNOWN = 900;

        /// <summary>
        /// The implementation is aware of multiple values for this Property.
        /// </summary>
        public static int MULTIPLE_VALUES = 10000;

        public ValueException()
            : base()
        {
        }

        public ValueException(int code, String message)
            : base(code, message)
        {
        }

        public ValueException(int code, Exception ex)
            : base(code, ex)
        {
        }
    }
}
