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

namespace Oddr.Models
{
    public class ODDRPropertyName : IPropertyName
    {

        private String localPropertyName;
        private String nameSpace;

        public ODDRPropertyName(String localPropertyName, String nameSpace)
        {
            this.localPropertyName = localPropertyName;
            this.nameSpace = nameSpace;
        }

        public string LocalPropertyName()
        {
            return this.localPropertyName;
        }

        public string Namespace()
        {
            return this.nameSpace;
        }
    }
}
