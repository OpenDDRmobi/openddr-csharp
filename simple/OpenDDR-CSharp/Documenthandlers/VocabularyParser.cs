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
using System.Xml.Linq;
using Oddr.Models.Vocabularies;
using System.IO;
using System.Xml;

namespace Oddr.Documenthandlers
{
    public class VocabularyParser
    {
        private const String ELEMENT_VOCABULARY_DESCRIPTION = "VocabularyDescription";
        private const String ELEMENT_ASPECTS = "Aspects";
        private const String ELEMENT_ASPECT = "Aspect";
        private const String ELEMENT_VARIABLES = "Variables";
        private const String ELEMENT_VARIABLE = "Variable";
        private const String ELEMENT_PROPERTIES = "Properties";
        private const String ELEMENT_PROPERTY = "Property";
        private const String ATTRIBUTE_PROPERTY_TARGET = "target";
        private const String ATTRIBUTE_PROPERTY_ASPECT_NAME = "name";
        private const String ATTRIBUTE_PROPERTY_ASPECT = "aspect";
        private const String ATTRIBUTE_PROPERTY_NAME = "name";
        private const String ATTRIBUTE_PROPERTY_VOCABULARY = "vocabulary";
        private const String ATTRIBUTE_PROPERTY_ID = "id";
        private const String ATTRIBUTE_PROPERTY_DATA_TYPE = "datatype";
        private const String ATTRIBUTE_PROPERTY_DATA_TYPE_CAMEL = "datatype";
        private const String ATTRIBUTE_PROPERTY_EXPR = "expr";
        private const String ATTRIBUTE_PROPERTY_ASPECTS = "aspects";
        private const String ATTRIBUTE_PROPERTY_DEFAULT_ASPECT = "defaultAspect";
        public Vocabulary vocabulary
        {
            private set;
            get;
        }
        private XDocument doc;

        /// <exception cref="ArgumentNullException">Thrown when...</exception>
        public VocabularyParser(Stream stream)
        {
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

            vocabulary = (from v in doc.Descendants(ELEMENT_VOCABULARY_DESCRIPTION)
                                     select new Vocabulary
                                     {
                                         vocabularyIRI = (string)v.Attribute(ATTRIBUTE_PROPERTY_TARGET).Value,
                                         aspects = (from aspects in v.Descendants(ELEMENT_ASPECTS)
                                                    select (string)aspects.Attribute(ELEMENT_ASPECT)).ToArray<string>(),
                                         properties = new Dictionary<string,VocabularyProperty>(),
                                         vocabularyVariables = new Dictionary<string,VocabularyVariable>(),
                                     }).First<Vocabulary>();

            XElement vocDescrXElement = doc.Descendants(ELEMENT_VOCABULARY_DESCRIPTION).First<XElement>();
            XElement propertiesXElement = vocDescrXElement.Descendants(ELEMENT_PROPERTIES).First<XElement>();

            VocabularyProperty[] vocabularyProperties = (from prop in propertiesXElement.Descendants(ELEMENT_PROPERTY)
                                                         //where prop.Attribute(ATTRIBUTE_PROPERTY_DATA_TYPE) != null
                                                         select new VocabularyProperty
                                                         {
                                                             aspects = prop.Attribute(ATTRIBUTE_PROPERTY_ASPECTS).Value.Replace(" ", "").Split(",".ToCharArray()),
                                                             defaultAspect = prop.Attribute(ATTRIBUTE_PROPERTY_DEFAULT_ASPECT).Value,
                                                             //expr = prop.Attribute(ATTRIBUTE_PROPERTY_EXPR).Value,
                                                             name = prop.Attribute(ATTRIBUTE_PROPERTY_NAME).Value,
                                                             type = prop.Attribute(ATTRIBUTE_PROPERTY_DATA_TYPE).Value,
                                                         }).ToArray<VocabularyProperty>();

            foreach (VocabularyProperty vp in vocabularyProperties)
            {
                vocabulary.properties.Add(vp.name, vp);
            }


            try
            {
                XElement variablesXElement = vocDescrXElement.Descendants(ELEMENT_VARIABLES).First<XElement>();

                VocabularyVariable[] vocabularyVariables = (from var in variablesXElement.Descendants(ELEMENT_VARIABLE)
                                                            select new VocabularyVariable
                                                            {
                                                                aspect = var.Attribute(ATTRIBUTE_PROPERTY_ASPECT).Value,
                                                                id = var.Attribute(ATTRIBUTE_PROPERTY_ID).Value,
                                                                name = var.Attribute(ATTRIBUTE_PROPERTY_NAME).Value,
                                                                vocabulary = var.Attribute(ATTRIBUTE_PROPERTY_VOCABULARY).Value,
                                                            }).ToArray<VocabularyVariable>();

                foreach (VocabularyVariable vv in vocabularyVariables)
                {
                    vocabulary.vocabularyVariables.Add(vv.id, vv);
                }
            }
            catch (Exception ex)
            {
                //TODO: Handle this Exception
            }
        }
    }
}
