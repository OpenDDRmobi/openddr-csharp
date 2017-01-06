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
using System;
using System.IO;
using System.Text;
//
namespace ClassifierClient
{
    /// <summary>
    ///  XML Parser
    /// </summary>
    /// <author>eberhard speer jr.</author>
    /// <remarks>OpenDDR Classifier Project .Net version 
    ///          ported from OpenDDR XMLParser.java</remarks>
    internal sealed class XmlParser
    {
        //
        private System.IO.StreamReader inStream;
        private char pre = '\0';

        #region "Properties"
        /// <summary>
        ///  Returns next XML tag in StreamReader
        /// </summary>
        /// <returns>String</returns>
        /// <remarks>-</remarks>
        public string NextTag
        {
            get
            {
                StringBuilder localBuilder = new StringBuilder();

                int i = 0;
                bool start = false;

                if (pre == '<')
                {
                    localBuilder.Append(pre);
                    pre = '\0';
                    start = true;
                }

                while ((i = inStream.Read()) != -1)
                {
                    char c = (char)i;
                    if (c == '<')
                    {
                        start = true;
                        localBuilder.Append(c);
                    }
                    else if (c == '>')
                    {
                        localBuilder.Append(c);
                        break;
                    }
                    else if (start)
                    {
                        localBuilder.Append(c);
                    }
                }

                return localBuilder.ToString();
            }
        }
        /// <summary>
        ///  Returns XML tag value from StreamReader
        /// </summary>
        /// <returns>String</returns>
        /// <remarks>-</remarks>
        public string TagValue
        {
            get
            {
                StringBuilder localBuilder = new StringBuilder();
                int i = 0;
                while ((i = inStream.Read()) != -1)
                {
                    char c = (char)i;
                    if (c == '<')
                    {
                        pre = '<';
                        break;
                    }
                    else
                    {
                        localBuilder.Append(c);
                    }
                }
                return localBuilder.ToString().Trim();
            }
        }
        #endregion

        #region "Constructor"
        /// <summary>
        ///  Prevent parameterless new
        /// </summary>
        /// <remarks>-</remarks>
        private XmlParser()
        {
            // Nice !
        }
        /// <summary>
        ///  New XmlParser for StreamReader
        /// </summary>
        /// <param name="stream">StreamReader</param>
        /// <remarks>-</remarks>
        public XmlParser(System.IO.StreamReader stream)
        {
            inStream = stream;
        }
        #endregion

        #region "Functions"
        /// <summary>
        ///  Returns Attribute (Device property) value of tag with name
        /// </summary>
        /// <param name="tag">XML tag</param>
        /// <param name="name">Attribute name</param>
        /// <returns>String</returns>
        /// <remarks>-</remarks>
        public static string getAttribute(string tag, string name)
        {
            int retpos = tag.ToLower().IndexOf(name.ToLower() + "=");
            if (retpos == -1)
            {
                return "";
            }
            string result = tag.Substring(retpos + name.Length + 1);
            if (result.StartsWith("\""))
            {
                result = result.Substring(1);
                int endpos = result.IndexOf("\"");
                if (endpos == -1)
                {
                    return "";
                }
                result = result.Substring(0, endpos);
            }
            else
            {
                int endpos = result.IndexOf(" ");
                if (endpos == -1)
                {
                    return "";
                }
                result = result.Substring(0, endpos);
            }
            return result;
        }
        #endregion
    }
}