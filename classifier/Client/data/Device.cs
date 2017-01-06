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
#endregion
//
using System.Collections.Generic;
//
namespace ClassifierClient
{
    /// <summary>
    ///  Device data
    /// </summary>
    /// <author>eberhard speer jr.</author>
    /// <remarks>Classifier Client Project .Net version<br />
    ///          ported from OpenDDR Device.java</remarks>
    internal sealed class Device
    {
        //
        private string builderType = string.Empty;
        private string deviceId = string.Empty;
        private string deviceParent = string.Empty;
        private Pattern pattern;
        private IDictionary<string, string> properties;

        #region "Properties"
        /// <summary>
        ///  Property dictionary
        /// </summary>
        /// <returns>IDictionary(Of String, String)</returns>
        /// <remarks>-</remarks>
        public IDictionary<string, string> Attributes
        {
            get { return properties; }
            set { properties = value; }
        }
        /// <summary>
        ///  Unique Id
        /// </summary>
        /// <returns>String</returns>
        /// <remarks>-</remarks>
        public string Id
        {
            get { return deviceId; }
            set { deviceId = value; }
        }
        /// <summary>
        ///  Unique Parent Id
        /// </summary>
        /// <returns>String</returns>
        /// <remarks>-</remarks>
        public string ParentId
        {
            get { return deviceParent; }
            set { deviceParent = value; }
        }
        /// <summary cref="Pattern">
        ///  Pattern collection
        /// </summary>
        /// <returns>Pattern</returns>
        /// <remarks>Collection of patterns to match with User-Agent string</remarks>
        public Pattern Patterns
        {
            get { return pattern; }
        }
        /// <summary>
        ///  Builder type
        /// </summary>
        /// <returns>String</returns>
        /// <remarks>used to destinguish between 'simple' and 'two-step' device builders</remarks>
        public string Type
        {
            get { return builderType; }
            set { builderType = value; }
        }
        #endregion

        #region "Constructor"
        /// <summary>
        ///  Default new Device
        /// </summary>
        /// <remarks>-</remarks>
        public Device()
        {
            pattern = new Pattern();
        }
        #endregion

        #region "Functions"
        /// <summary>
        ///  To String override
        /// </summary>
        /// <returns>String</returns>
        /// <remarks>-</remarks>
        public override string ToString()
        {
            return string.Format(Constants.DEVICE_TOSTRING_FORMAT, deviceId, deviceParent, builderType, pattern.ToString(), properties.ToString());
        }
        #endregion
    }
}