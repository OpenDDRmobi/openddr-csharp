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
using Oddr.Builders.Devices;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;

namespace Oddr.Documenthandlers
{
    public class DeviceBuilderParser
    {
        private const string ELEMENT_BUILDERS = "Builders";
        private const string BUILDER_DEVICE = "builder"; //Java source: private Object BUILDER_DEVICE = "builder";
        private const String ELEMENT_DEVICE = "device";
        private const String ELEMENT_PROPERTY = "property";
        private const String ELEMENT_LIST = "list";
        private const String ELEMENT_VALUE = "value";
        private const String ATTRIBUTE_DEVICE_ID = "id";
        private const string ATTRIBUTE_CLASS = "class";
        private List<IDeviceBuilder> builders;
        private XDocument doc;
        private Dictionary<string, string> deviceBuilderClassMapper;


        public DeviceBuilderParser(Stream stream)
        {
            Init(stream);
            this.builders = new List<IDeviceBuilder>();
        }

        public DeviceBuilderParser(Stream stream, List<IDeviceBuilder> builders)
        {
            Init(stream);
            this.builders = builders;
        }

        /// <exception cref="System.ArgumentNullException">Thrown when stream is null</exception>
        private void Init(Stream stream)
        {
            try
            {
                SetStream(stream);
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException(ex.Message, ex);
            }

            deviceBuilderClassMapper = new Dictionary<string, string>();
            deviceBuilderClassMapper.Add("mobi.openddr.simple.builder.device.AndroidDeviceBuilder", "Oddr.Builders.Devices.AndroidDeviceBuilder");
            deviceBuilderClassMapper.Add("mobi.openddr.simple.builder.device.SymbianDeviceBuilder", "Oddr.Builders.Devices.SymbianDeviceBuilder");
            deviceBuilderClassMapper.Add("mobi.openddr.simple.builder.device.WinPhoneDeviceBuilder", "Oddr.Builders.Devices.WinPhoneDeviceBuilder");
            deviceBuilderClassMapper.Add("mobi.openddr.simple.builder.device.IOSDeviceBuilder", "Oddr.Builders.Devices.IOSDeviceBuilder");
            deviceBuilderClassMapper.Add("mobi.openddr.simple.builder.device.SimpleDeviceBuilder", "Oddr.Builders.Devices.SimpleDeviceBuilder");
            deviceBuilderClassMapper.Add("mobi.openddr.simple.builder.device.TwoStepDeviceBuilder", "Oddr.Builders.Devices.TwoStepDeviceBuilder");
            deviceBuilderClassMapper.Add("mobi.openddr.simple.builder.device.BotDeviceBuilder", "Oddr.Builders.Devices.SimpleDeviceBuilder");
            deviceBuilderClassMapper.Add("mobi.openddr.simple.builder.device.DesktopOSDeviceBuilder", "Oddr.Builders.Devices.SimpleDeviceBuilder");
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

        /// <exception cref="Exception">Thrown when...</exception>
        public void Parse()
        {
            if (doc == null)
            {
                throw new Exception("Input stream is not valid");
            }
            //Console.WriteLine("Parsing...");
            BuilderWrapper[] buildersWrapper = (from b in doc.Descendants(ELEMENT_BUILDERS).Descendants(BUILDER_DEVICE)
                                                select new BuilderWrapper
                                                {
                                                    attributeClass = b.Attribute(ATTRIBUTE_CLASS).Value,
                                                    devices = (from d in b.Descendants(ELEMENT_DEVICE)
                                                               select new DeviceWrapper
                                                               {
                                                                   id = d.Attribute(ATTRIBUTE_DEVICE_ID).Value,
                                                                   values = d.Element(ELEMENT_LIST).Descendants(ELEMENT_VALUE).Select(x => x.Value).ToList(),
                                                               }).ToList<DeviceWrapper>(),
                                                }).ToArray<BuilderWrapper>();

            foreach (BuilderWrapper bw in buildersWrapper)
            {
                IDeviceBuilder deviceBuilderInstance = null;

                try
                {
                    Type builderType = Type.GetType(deviceBuilderClassMapper[bw.attributeClass], true);
                    foreach (IDeviceBuilder deviceBuilder in builders)
                    {
                        if (deviceBuilder.GetType().Equals(builderType))
                        {
                            deviceBuilderInstance = deviceBuilder;
                        }
                    }

                    if (deviceBuilderInstance == null)
                    {
                        deviceBuilderInstance = (IDeviceBuilder)Activator.CreateInstance(builderType);
                        builders.Add(deviceBuilderInstance);
                    }

                    foreach (DeviceWrapper d in bw.devices)
                    {
                        deviceBuilderInstance.PutDevice(d.id, d.values);
                    }
                }
                catch (ArgumentNullException ane)
                {
                    throw new ArgumentNullException("Argument is null", ane);
                }
                catch (TargetInvocationException tie)
                {
                    throw new ArgumentException("Can not instantiate class: {0} described in device builder document due to constructor exception", deviceBuilderClassMapper[bw.attributeClass]);
                }
                catch (TypeLoadException tle)
                {
                    throw new ArgumentException("Can not find class: {0} described in device builder document", deviceBuilderClassMapper[bw.attributeClass]);
                }
                catch (IOException ioe)
                {
                    throw new ArgumentException("Can not find file: {0} described in device builder document", deviceBuilderClassMapper[bw.attributeClass]);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Can not instantiate class: {0} described in device builder document", deviceBuilderClassMapper[bw.attributeClass]);
                }
            }
        }

        private class BuilderWrapper
        {
            public string attributeClass;
            public List<DeviceWrapper> devices;
        }

        private class DeviceWrapper
        {
            public string id;
            public List<string> values;
        }

        public IDeviceBuilder[] DeviceBuilders()
        {
            return builders.ToArray<IDeviceBuilder>();
        }
    }
}
