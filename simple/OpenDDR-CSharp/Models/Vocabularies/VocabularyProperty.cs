﻿/**
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

namespace Oddr.Models.Vocabularies
{
    public class VocabularyProperty
    {
        public String[] aspects
        {
            get;
            set;
        }
        public String defaultAspect
        {
            get;
            set;
        }
        public String expr
        {
            get;
            set;
        }
        public String name
        {
            get;
            set;
        }
        public String type
        {
            get;
            set;
        }
    }
}