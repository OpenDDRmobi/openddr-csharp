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
#endregion // Header
//
using System.IO;
//
namespace ClassifierClient
{
    /// <summary>
    ///  Load XML resources from local file
    /// </summary>
    /// <remarks>-</remarks>
    internal sealed class FileLoader //: ILoader
    {
        //
        private long resLength { get; set; }
        private StreamReader resReader { get; set; }
        private string resUrl { get; set; }
        //
        #region "Properties"
        /// <summary>
        ///  Resource path
        /// </summary>
        /// <returns>String</returns>
        /// <remarks>-</remarks>
        public string Path
        {
            get { return resUrl; }
        }
        /// <summary>
        ///  Returns the resource file length
        /// </summary>
        /// <returns>Long</returns>
        /// <remarks>file length</remarks>
        public long ResponseLength
        {
            get { return resLength; }
        }
        /// <summary>
        ///  Reader
        /// </summary>
        /// <returns>StreamReader</returns>
        /// <remarks>-</remarks>
        public StreamReader Reader
        {
            get { return resReader; }
        }
        #endregion // Properties

        #region "Constructor"
        /// <summary>
        ///  Load resource for path string
        /// </summary>
        /// <param name="filePath">path and file name of resource file</param>
        /// <exception cref="ArgumentException">thrown when file does not exist</exception>
        /// <remarks>-</remarks>
        public FileLoader(string filePath)
        {
            resUrl = filePath.Trim();
            if (File.Exists(resUrl))
            {
                resLength = new FileInfo(filePath).Length;
                resReader = new StreamReader(resUrl);
            }
            else
            {
                throw new System.ArgumentException(string.Format(Constants.FILE_ERROR_FORMAT, resUrl));
            }
        }
        #endregion ' Constructor
    }
}