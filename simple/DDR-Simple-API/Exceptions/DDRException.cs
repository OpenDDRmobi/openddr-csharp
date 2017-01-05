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
    /// It is the superclass of all DDR Simple API exceptions other than SystemException.
    /// Implementations should raise subclasses of DDRException, they should not raise this exception directly.
    /// </summary>
    public class DDRException : Exception
    {
        public const long serialVersionUID = 2618094065573111548L;
        /// <summary>
        /// This code may be used by implementations to create custom error codes. All implementation specific codes must be greater than this value.
        /// Implementations may define specific codes for different kinds of failures during initialization.
        /// </summary>
        public static int IMPLEMENTATION_ERROR = 65536;

        public int code
        {
            get;
            protected set;
        }

        public DDRException()
            : base()
        {
        }

        public DDRException(int code, String message) 
            : base(message)
        {
            this.code = code;
        }

        public DDRException(int code, Exception ex)
            : base("", ex)
        {
            this.code = code;
        }

        public String GetMessage()
        {
            return base.Message;
        }
    }
}
