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
using System.IO;
using Oddr.Vocabularies;
using Oddr.Models.Devices;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using W3c.Ddr.Exceptions;

namespace Oddr.Documenthandlers
{
    class DeviceDatasourceParser
    {
        private const String PROPERTY_ID = "id";
        private const String ELEMENT_DEVICES = "Devices";
        private const String ELEMENT_DEVICE = "device";
        private const String ELEMENT_PROPERTY = "property";
        private const String ATTRIBUTE_DEVICE_ID = "id";
        private const String ATTRIBUTE_DEVICE_PARENT_ID = "parentId";
        private const String ATTRIBUTE_PROPERTY_NAME = "name";
        private const String ATTRIBUTE_PROPERTY_VALUE = "value";
        private VocabularyHolder vocabularyHolder;
        private XDocument doc;
        public Dictionary<String, Device> devices
        {
            private set;
            get;
        }
        public bool patching
        {
            set;
            private get;
        }

        /// <exception cref="System.ArgumentNullException">Thrown when stream is null</exception>
        public DeviceDatasourceParser(Stream stream)
        {
            try
            {
                Init(stream);
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException(ex.Message, ex);
            }
            this.devices = new Dictionary<string, Device>();
        }

        /// <exception cref="System.ArgumentNullException">Thrown when stream is null</exception>
        public DeviceDatasourceParser(Stream stream, Dictionary<string, Device> devices)
        {
            try
            {
                Init(stream);
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException(ex.Message, ex);
            }
            this.devices = devices;
        }

        /// <exception cref="System.ArgumentNullException">Thrown when stream is null</exception>
        public DeviceDatasourceParser(Stream stream, Dictionary<string, Device> devices, VocabularyHolder vocabularyHolder)
        {
            try
            {
                Init(stream);
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException(ex.Message, ex);
            }
            this.devices = devices;
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

        /// <exception cref="System.ArgumentNullException">Thrown when stream is null</exception>
        private void Init(Stream stream)
        {
            this.patching = false;
            try
            {
                SetStream(stream);
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException(ex.Message, ex);
            }

        }

        /// <exception cref="System.ArgumentNullException">Thrown when stream is null</exception>
        public void SetStream(Stream stream)
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

        /// <exception cref="Exception">Thrown when input stream is not valid</exception>
        public void Parse()
        {
            if (doc == null)
            {
                throw new Exception("Input stream is not valid");
            }

            DeviceWrapper[] deviceWrapperArray = (from d in doc.Descendants(ELEMENT_DEVICES).Descendants(ELEMENT_DEVICE)
                                                  where d.Attribute(ATTRIBUTE_DEVICE_PARENT_ID) != null
                                                  select new DeviceWrapper
                                                  {
                                                      id = d.Attribute(ATTRIBUTE_DEVICE_ID).Value,
                                                      parentId = d.Attribute(ATTRIBUTE_DEVICE_PARENT_ID).Value,
                                                      properties = (from prop in d.Descendants(ELEMENT_PROPERTY)
                                                                    select new StringPair
                                                                    {
                                                                        key = prop.Attribute(ATTRIBUTE_PROPERTY_NAME).Value,
                                                                        value = prop.Attribute(ATTRIBUTE_PROPERTY_VALUE).Value,
                                                                    }).ToArray<StringPair>(),
                                                  }).ToArray<DeviceWrapper>();

            DeviceWrapper[] deviceWrapperArray2 = (from d in doc.Descendants(ELEMENT_DEVICES).Descendants(ELEMENT_DEVICE)
                                                   where d.Attribute(ATTRIBUTE_DEVICE_PARENT_ID) == null
                                                   select new DeviceWrapper
                                                   {
                                                       id = d.Attribute(ATTRIBUTE_DEVICE_ID).Value,
                                                       properties = (from prop in d.Descendants(ELEMENT_PROPERTY)
                                                                     select new StringPair
                                                                     {
                                                                         key = prop.Attribute(ATTRIBUTE_PROPERTY_NAME).Value,
                                                                         value = prop.Attribute(ATTRIBUTE_PROPERTY_VALUE).Value,
                                                                     }).ToArray<StringPair>(),
                                                   }).ToArray<DeviceWrapper>();

            deviceWrapperArray = deviceWrapperArray.Concat(deviceWrapperArray2).ToArray();

            foreach (DeviceWrapper dw in deviceWrapperArray)
            {
                Device device = dw.GetDevice(vocabularyHolder);

                Device existDevice = null;
                if (devices.TryGetValue(device.id, out existDevice))
                {
                    if (patching)
                    {
                        existDevice.PutPropertiesMap(device.properties);
                        continue;
                    }
                    else
                    {
                        //TODO: WARNING already present
                    }
                }

                try
                {
                    device.properties.Add(PROPERTY_ID, device.id);
                }
                catch (ArgumentException ae)
                {
                    //Console.WriteLine(this.GetType().FullName + " " + PROPERTY_ID + " already exist in device " + device.id);
                }

                try
                {
                    devices.Add(device.id, device);
                }
                catch (ArgumentException ae)
                {
                    //Console.WriteLine(this.GetType().FullName + " " + device.id + " already exist in device " + device.id);
                    devices.Remove(device.id);
                    devices.Add(device.id, device);
                }
            }

        }

        private class DeviceWrapper
        {
            public string id;
            public string parentId;
            public StringPair[] properties;

            public Device GetDevice(VocabularyHolder vocabularyHolder)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();

                if (vocabularyHolder != null)
                {
                    foreach (StringPair sp in properties)
                    {
                        try
                        {
                            //vocabularyHolder.ExistProperty(sp.key, ODDRService.ASPECT_DEVICE, ODDRVocabularyService.ODDR_LIMITED_VOCABULARY_IRI);
                            if (vocabularyHolder.ExistProperty(sp.key, ODDRService.ASPECT_DEVICE, ODDRVocabularyService.ODDR_LIMITED_VOCABULARY_IRI) != null)
                            {
                                //dic.Add(sp.key, sp.value);
                                dic[sp.key] = sp.value;
                            }
                        }
                        //catch (NameException ex)
                        //{
                        //    Console.WriteLine(ex.Message);
                        //    //property non loaded
                        //}
                        catch (ArgumentException ae)
                        {
                            //Console.WriteLine(this.GetType().FullName + " " + sp.key + " already present in device " + id);
                        }
                    }
                }
                else
                {
                    foreach (StringPair sp in properties)
                    {
                        //dic.Add(sp.key, sp.value);
                        dic[sp.key] = sp.value;
                    }
                }

                Device d = new Device();
                d.id = this.id;
                d.parentId = this.parentId;
                d.PutPropertiesMap(dic);

                return d;
            }
        }

        private class StringPair
        {
            public string key;
            public string value;
        }
    }
}
