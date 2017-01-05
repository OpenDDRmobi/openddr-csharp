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
    /// This is a subclass of DDRException and represents an error during the initialization phase of the Simple API. It is thrown only by the initialize method of the Service interface and the newService method of the ServiceFactory class.
    /// </summary>
    public class InitializationException : DDRException
    {
        /// <summary>
        /// There was a problem during initialization. Implementations may define specific codes for different kinds of failures during initialization.
        /// </summary>
        public static int INITIALIZATION_ERROR = 300;

        public InitializationException()
            : base()
        {
        }

        public InitializationException(int code, String message)
            : base(code, message)
        {
        }

        public InitializationException(int code, Exception ex)
            : base(code, ex)
        {
        }

    }
}
