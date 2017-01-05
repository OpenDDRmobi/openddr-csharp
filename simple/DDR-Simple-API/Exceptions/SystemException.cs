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
    /// This exception, a subclass of System.Exception, Is thrown by DDR Simple API implementations when they encounter unrecoverable errors.
    /// </summary>
    public class SystemException : Exception
    {
        /// <summary>
        /// A method has been passed an illegal or inappropriate argument - a null argument where it is not allowed, for example.
        /// </summary>
        public static int ILLEGAL_ARGUMENT = 400;

        /// <summary>
        /// The implementation cannot continue with the processing of the current request due to an unexpected failure - disconnection from a database, for example.
        /// </summary>
        public static int CANNOT_PROCEED = 500;

        protected int code
        {
            get;
            private set;
        }

        public SystemException()
            : base()
        {
        }

        public SystemException(int code, String message)
            : base(message)
        {
            this.code = code;
        }

        public SystemException(int code, Exception ex)
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
