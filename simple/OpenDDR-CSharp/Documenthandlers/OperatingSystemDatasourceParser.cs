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
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Oddr.Models.OS;
using Oddr.Vocabularies;
using W3c.Ddr.Exceptions;

namespace Oddr.Documenthandlers
{
    public class OperatingSystemDatasourceParser
    {
        private const string ELEMENT_OPERATING_SYSTEM = "OperatingSystem";
        private const String ELEMENT_OPERATING_SYSTEM_DESCRIPTION = "operatingSystem";
        private const String ELEMENT_PROPERTY = "property";
        private const String ATTRIBUTE_OS_ID = "id";
        private const String ATTRIBUTE_PROPERTY_NAME = "name";
        private const String ATTRIBUTE_PROPERTY_VALUE = "value";
        private XDocument doc;
        private VocabularyHolder vocabularyHolder;
        public SortedDictionary<String, Oddr.Models.OS.OperatingSystem> operatingSystems
        {
            private set;
            get;
        }

        public OperatingSystemDatasourceParser(Stream stream)
        {
            Init(stream);
        }

        public OperatingSystemDatasourceParser(Stream stream, VocabularyHolder vocabularyHolder)
        {
            Init(stream);
            try
            {
                vocabularyHolder.ExistVocabulary(ODDRVocabularyService.ODDR_LIMITED_VOCABULARY_IRI);
                this.vocabularyHolder = vocabularyHolder;
            }
            catch (Exception ex)
            {
                this.vocabularyHolder = null;
            }

        }

        private void Init(Stream stream)
        {
            this.operatingSystems = new SortedDictionary<string, Oddr.Models.OS.OperatingSystem>(StringComparer.Ordinal);
            XmlReader reader = null;
            try
            {
                reader = XmlReader.Create(stream);
                doc = XDocument.Load(reader);
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException(ex.Message, ex);
            }
            finally
            {
                if (reader != null)
                {
                    ((IDisposable)reader).Dispose();
                }
            }
        }

        /// <exception cref="Exception">Thrown when...</exception>
        public void Parse()
        {
            if (doc == null)
            {
                throw new Exception("Input stream is not valid");
            }

            OperatingSystemWrapper[] osWrapperArray = (from os in doc.Descendants(ELEMENT_OPERATING_SYSTEM).Descendants(ELEMENT_OPERATING_SYSTEM_DESCRIPTION)
                                                       select new OperatingSystemWrapper
                                             {
                                                 id = os.Attribute(ATTRIBUTE_OS_ID).Value,
                                                 properties = (from prop in os.Descendants(ELEMENT_PROPERTY)
                                                               select new StringPair
                                                               {
                                                                   key = prop.Attribute(ATTRIBUTE_PROPERTY_NAME).Value,
                                                                   value = prop.Attribute(ATTRIBUTE_PROPERTY_VALUE).Value,
                                                               }).ToArray<StringPair>(),
                                             }).ToArray<OperatingSystemWrapper>();

            foreach (OperatingSystemWrapper osw in osWrapperArray)
            {
                Oddr.Models.OS.OperatingSystem os = osw.GetOperatingSystem(vocabularyHolder);
                operatingSystems.Add(osw.id, os);
            }
        }

        private class OperatingSystemWrapper
        {
            public string id;
            public StringPair[] properties;

            public Oddr.Models.OS.OperatingSystem GetOperatingSystem(VocabularyHolder vocabularyHolder)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();

                if (vocabularyHolder != null)
                {
                    foreach (StringPair sp in properties)
                    {
                        try
                        {
                            //vocabularyHolder.ExistProperty(sp.key, ODDRService.ASPECT_OPERATIVE_SYSTEM, ODDRVocabularyService.ODDR_LIMITED_VOCABULARY_IRI);
                            if (vocabularyHolder.ExistProperty(sp.key, ODDRService.ASPECT_OPERATIVE_SYSTEM, ODDRVocabularyService.ODDR_LIMITED_VOCABULARY_IRI) != null)
                            {
                                dic.Add(sp.key, sp.value);
                            }
                        }
                        //catch (NameException ex)
                        //{
                        //    //property non loaded
                        //}
                        catch (ArgumentException ae)
                        {
                            //Console.WriteLine(this.GetType().FullName + " " + sp.key + " already present!!!");
                        }
                    }
                }
                else
                {
                    foreach (StringPair sp in properties)
                    {
                        dic.Add(sp.key, sp.value);
                    }
                }

                return new Oddr.Models.OS.OperatingSystem(dic);
            }
        }

        private class StringPair
        {
            public string key;
            public string value;
        }
    }
}
