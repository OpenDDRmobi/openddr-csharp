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
    /// This is a subclass of DDRException and is thrown when it is detected that the name of a Property or Aspect or vocabulary IRI is in error. The exception code, when set, indicates the nature of the error.
    /// A name of a Property or Aspect or a vocabulary IRI are in error when they are not syntactically valid or are not supported by the implementation.
    /// </summary>
    public class NameException : DDRException
    {
        /// <summary>
        /// The name of a Property is in error
        /// </summary>
        public static int PROPERTY_NOT_RECOGNIZED = 100;

        /// <summary>
        /// The name of an Aspect is in error
        /// </summary>
        public static int VOCABULARY_NOT_RECOGNIZED = 200;

        /// <summary>
        /// A vocabulary IRI is in error
        /// </summary>
        public static int ASPECT_NOT_RECOGNIZED = 800;

        public NameException()
            : base()
        {
        }

        public NameException(int code, String message)
            : base(code, message)
        {
        }

        public NameException(int code, Exception ex)
            : base(code, ex)
        {
        }
    }
}
