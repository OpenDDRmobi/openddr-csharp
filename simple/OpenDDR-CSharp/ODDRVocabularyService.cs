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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oddr.Vocabularies;
using W3c.Ddr.Models;
using Oddr.Models.Vocabularies;
using System.IO;
using W3c.Ddr.Exceptions;
using Oddr.Documenthandlers;

namespace Oddr
{
    /// <summary>
    /// The ODDRVocabularyService class is used by ODDRService at initialization time in order to parse the vocabularies xml files and to instantiate the vocabularyHolder.
    /// </summary>
    class ODDRVocabularyService
    {
        public const String DDR_CORE_VOCABULARY_PATH_PROP = "ddr.vocabulary.core.path";
        public const String ODDR_VOCABULARY_PATH_PROP = "oddr.vocabulary.path";
        public const String ODDR_LIMITED_VOCABULARY_PATH_PROP = "oddr.limited.vocabulary.path";
        public const String DDR_CORE_VOCABULARY_STREAM_PROP = "ddr.vocabulary.core.stream";
        public const String ODDR_VOCABULARY_STREAM_PROP = "oddr.vocabulary.stream";
        public const String ODDR_LIMITED_VOCABULARY_STREAM_PROP = "oddr.limited.vocabulary.stream";
        public const String ODDR_LIMITED_VOCABULARY_IRI = "limitedVocabulary";

        public VocabularyHolder vocabularyHolder
        {
            private set;
            get;
        }

        /// <summary>
        /// Initialization funcion. It is called by ODDRService at initialization time in order to populate vocabulary holder.
        /// </summary>
        /// <param name="props">Properties object holding the configuration properties.</param>
        /// <exception cref="InitializationException">Throws when...</exception>
        public void Initialize(Properties props)
        {
            Dictionary<String, Vocabulary> vocabularies = new Dictionary<String, Vocabulary>();

            String ddrCoreVocabularyPath = props.GetProperty(DDR_CORE_VOCABULARY_PATH_PROP);
            String oddrVocabularyPath = props.GetProperty(ODDR_VOCABULARY_PATH_PROP);

            Stream ddrCoreVocabulayStream = null;
            Stream[] oddrVocabularyStream = null;
            try {
                ddrCoreVocabulayStream = props.Get(DDR_CORE_VOCABULARY_STREAM_PROP) as Stream;
            } catch (Exception ex) {
                ddrCoreVocabulayStream = null;
            }
            try {
                oddrVocabularyStream = props.Get(ODDR_VOCABULARY_STREAM_PROP) as Stream[];
            } catch (Exception ex) {
                oddrVocabularyStream = null;
            }

            if ((string.IsNullOrEmpty(ddrCoreVocabularyPath)) && ddrCoreVocabulayStream == null) {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new ArgumentException("Can not find property " + DDR_CORE_VOCABULARY_PATH_PROP));
            }

            if ((string.IsNullOrEmpty(oddrVocabularyPath)) && oddrVocabularyStream == null) {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new ArgumentException("Can not find property " + ODDR_VOCABULARY_PATH_PROP));
            }

            VocabularyParser vocabularyParser = null;
            Vocabulary vocabulary = null;

            if (ddrCoreVocabulayStream != null)
            {
                vocabularyParser = ParseVocabularyFromStream(DDR_CORE_VOCABULARY_STREAM_PROP, ddrCoreVocabulayStream);
            }
            else
            {
                vocabularyParser = ParseVocabularyFromPath(DDR_CORE_VOCABULARY_PATH_PROP, ddrCoreVocabularyPath);
            }
            vocabulary = vocabularyParser.vocabulary;
            vocabularies.Add(vocabulary.vocabularyIRI, vocabulary);

            if (oddrVocabularyStream != null)
            {
                foreach (Stream stream in oddrVocabularyStream)
                {
                    vocabularyParser = ParseVocabularyFromStream(ODDR_VOCABULARY_STREAM_PROP, stream);
                    vocabulary = vocabularyParser.vocabulary;
                    vocabularies.Add(vocabulary.vocabularyIRI, vocabulary);
                }
            }
            else
            {
                String[] oddrVocabularyPaths = oddrVocabularyPath.Split(",".ToCharArray());
                foreach (string p in oddrVocabularyPaths)
                {
                    p.Trim();
                }

                foreach (String oddVocabularyString in oddrVocabularyPaths)
                {
                    vocabularyParser = ParseVocabularyFromPath(ODDR_VOCABULARY_PATH_PROP, oddVocabularyString);
                    vocabulary = vocabularyParser.vocabulary;
                    vocabularies.Add(vocabulary.vocabularyIRI, vocabulary);
                }
            }

            String oddrLimitedVocabularyPath = props.GetProperty(ODDR_LIMITED_VOCABULARY_PATH_PROP);
            Stream oddrLimitedVocabularyStream = props.Get(ODDR_LIMITED_VOCABULARY_STREAM_PROP) as Stream;

            if (oddrLimitedVocabularyStream != null) {
                vocabularyParser = ParseVocabularyFromStream(ODDR_LIMITED_VOCABULARY_STREAM_PROP, oddrLimitedVocabularyStream);
                vocabulary = vocabularyParser.vocabulary;
                vocabularies.Add(ODDR_LIMITED_VOCABULARY_IRI, vocabulary);
            } else {
               if (!string.IsNullOrEmpty(oddrLimitedVocabularyPath)) {
                   vocabularyParser = ParseVocabularyFromPath(ODDR_LIMITED_VOCABULARY_PATH_PROP, oddrLimitedVocabularyPath);
                   vocabulary = vocabularyParser.vocabulary;
                   vocabularies.Add(ODDR_LIMITED_VOCABULARY_IRI, vocabulary);
                }
            }
            //vocabulary = vocabularyParser.vocabulary;
            //vocabularies.Add(ODDR_LIMITED_VOCABULARY_IRI, vocabulary);

            vocabularyHolder = new VocabularyHolder(vocabularies);

            vocabularyParser = null;
            vocabularies = null;

        }

        /// <summary>
        /// Parse a vocabulary from a specified path.
        /// </summary>
        /// <param name="prop">The property name in Property class identifying the vocabulary to parse.</param>
        /// <param name="path">The path of the vocabulary to parse.</param>
        /// <returns>Return a VocabularyParser containing the parsed vocabulary model.</returns>
        /// <exception cref="InitializationException">Throws when...</exception>
        private VocabularyParser ParseVocabularyFromPath(String prop, String path)
        {
            VocabularyParser vocabularyParser;
            FileStream stream = null;

            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            }
            catch (IOException ex) {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new ArgumentException("Can not open " + prop + " : " + path));
            }

            try
            {
                vocabularyParser = new VocabularyParser(stream);

            }
            catch (ArgumentNullException ex) {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new InvalidOperationException("Can not instantiate VocabularyParser(Stream stream)"));

            }

            try
            {
                vocabularyParser.Parse();

            }
            catch (Exception ex) {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new Exception("Can not parse document: " + path));
            }

            stream.Close();
            return vocabularyParser;
        }

        /// <summary>
        /// Parse a vocabulary from a specified input stream.
        /// </summary>
        /// <param name="prop">The property name in Property class identifying the vocabulary to parse.</param>
        /// <param name="inputStream">The input stream of the vocabulary to parse.</param>
        /// <returns>Return a VocabularyParser containing the parsed vocabulary model.</returns>
        /// <exception cref="InitializationException">Throws when...</exception>
        private VocabularyParser ParseVocabularyFromStream(String prop, Stream inputStream)
        {
            VocabularyParser vocabularyParser;
            try
            {
                vocabularyParser = new VocabularyParser(inputStream);
            }
            catch (ArgumentNullException ex)
            {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new InvalidOperationException("Can not instantiate VocabularyParser(Stream stream)"));

            }

            try
            {
                vocabularyParser.Parse();

            }
            catch (Exception ex) {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new Exception("Can not parse document in property: " + prop));

            }

            inputStream.Close();
            return vocabularyParser;
        }
    }
}
