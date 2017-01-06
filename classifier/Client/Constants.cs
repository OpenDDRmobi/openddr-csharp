#region "Header"
/*
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
#endregion ' Header
//
namespace ClassifierClient
{
    public class Constants
    {
        public const string APP_NAME = "ClassifierClient";
        // data files
        public const string BUILDER_DATA = "BuilderDataSource.xml";
        public const string BUILDER_DATA_PATCH = "BuilderDataSourcePatch.xml";
        public const string DEVICE_DATA = "DeviceDataSource.xml";

        public const string DEVICE_DATA_PATCH = "DeviceDataSourcePatch.xml";

        //public const string DEVICE_UA = "mobi.openddr.classifier.client";
        public const string DEVICE_TOSTRING_FORMAT = "Id='{0}', ParentId='{1}', Type='{2}', Pattern={3}, Attributes={4}";
        public const string HTTP_PREFIX = "http";
        public const string RELEASE_VERSION = "1.0";
        public const string SIMPLE = "simple";
        public const string USER_AGENT_SPLIT = " |-|_|/|\\\\|\\[|\\]|\\(|\\)|;";

        public const string VERSION_FORMAT = "Version : {0}, Build : {1}";
        public const string CONFIG_ERROR_FORMAT = "Error in configuration file '{0}' : {1}.";
        public const string CONFIG_ERROR_CONN_FORMAT = "Error in configuration file '{0}' : ConnectionString entry for Classifier is missing.";
        public const string FILE_ERROR_FORMAT = "File '{0}' not found.";
        public const string WEB_ERROR_FORMAT = "Web exception : {0}.";
    }
}