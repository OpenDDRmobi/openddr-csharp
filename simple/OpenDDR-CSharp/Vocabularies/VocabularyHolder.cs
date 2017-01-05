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
using Oddr.Caches;
using Oddr.Models.Vocabularies;
using W3c.Ddr.Exceptions;
using System.Diagnostics;

namespace Oddr.Vocabularies
{
    public class VocabularyHolder
    {
        private Dictionary<String, Vocabulary> vocabularies = null;
        private ICache vocabularyPropertyCache = new Cache();

        public VocabularyHolder(Dictionary<String, Vocabulary> vocabularies)
        {
            this.vocabularies = new Dictionary<string,Vocabulary>(vocabularies);
        }

        /// <exception cref="W3c.Ddr.Exceptions.NameException">Thrown when...</exception>
        public void ExistVocabulary(String vocabularyIRI)
        {
            Vocabulary value = null;
            if (!(vocabularies.TryGetValue(vocabularyIRI, out value)))
            {
                throw new NameException(NameException.VOCABULARY_NOT_RECOGNIZED, "unknow \"" + vocabularyIRI + "\" vocabulary");
            }
        }

        /// <exception cref="W3c.Ddr.Exceptions.NameException">Thrown when an aspect or property or vocabulary is not recognized</exception>
        public VocabularyProperty ExistProperty(String propertyName, String aspect, String vocabularyIRI, bool throwsException)
        {
            String realAspect = aspect;
            VocabularyProperty vocabularyProperty = (VocabularyProperty)vocabularyPropertyCache.GetCachedElement(propertyName + aspect + vocabularyIRI);

            if (vocabularyProperty == null)
            {
                Vocabulary vocabulary = new Vocabulary();
                if (vocabularies.TryGetValue(vocabularyIRI, out vocabulary))
                {
                    Dictionary<String, VocabularyProperty> propertyMap = vocabulary.properties;

                    if (propertyMap.TryGetValue(propertyName, out vocabularyProperty))
                    {
                        if (realAspect != null && realAspect.Trim().Length > 0)
                        {
                            if (vocabularyProperty.aspects.Contains(realAspect))
                            {
                                vocabularyPropertyCache.SetCachedElement(propertyName + aspect + vocabularyIRI, vocabularyProperty);
                                return vocabularyProperty;

                            }
                            else
                            {
                                if (throwsException)
                                {
                                    throw new NameException(NameException.ASPECT_NOT_RECOGNIZED, "unknow \"" + realAspect + "\" aspect");
                                }
                                return null;
                            }

                        }
                        else
                        {
                            return vocabularyProperty;
                        }

                    }
                    else
                    {
                        if (throwsException)
                        {
                            throw new NameException(NameException.PROPERTY_NOT_RECOGNIZED, "unknow \"" + propertyName + "\" property");
                        }
                        return null;
                    }

                }
                else
                {
                    if (throwsException)
                    {
                        throw new NameException(NameException.VOCABULARY_NOT_RECOGNIZED, "unknow \"" + vocabularyIRI + "\" vacabulary");
                    }
                    return null;
                }

            }
            else
            {
                return vocabularyProperty;
            }
        }

        public VocabularyProperty ExistProperty(String propertyName, String aspect, String vocabularyIRI)
        {
            return ExistProperty(propertyName, aspect, vocabularyIRI, false);
        }

        public Dictionary<String, Vocabulary> GetVocabularies() {
            return vocabularies;
        }
    }
}
