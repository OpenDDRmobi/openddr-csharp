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
using System.Text;
//
namespace ClassifierClient
{
    /// <summary>
    ///  Device Pattern data
    /// </summary>
    /// <author>eberhard speer jr.</author>
    /// <remarks>OpenDDR Project .Net version 
    ///          ported from OpenDDR Classifier Pattern.java</remarks>
    internal sealed class Pattern
    {
        //
        private IList<IList<string>> patternList;

        #region "Properties"
        /// <summary>
        ///  List of Patterns which <em>all</em> must occur in User-Agent string for a match
        /// </summary>
        /// <remarks>-</remarks>
        public IList<string> AndPattern
        {
            set { patternList.Add(value); }
        }
        /// <summary>
        ///  List of Patterns of which <em>at least one</em> must occur in User-Agent string for a match
        /// </summary>
        /// <remarks>-</remarks>
        public IList<string> OrPattern
        {
            set
            {
                foreach (string patternString in value)
                {
                    AddPattern = patternString;
                }
            }
        }
        /// <summary>
        ///  List of Patterns to match with User-Agent string
        /// </summary>
        /// <remarks>-</remarks>
        public string AddPattern
        {
            set
            {
                IList<string> subList = new List<string>();
                subList.Add(value);
                patternList.Add(subList);
            }
        }
        /// <summary>
        ///  List of Patterns Lists to match with User-Agent string
        /// </summary>
        /// <returns>IList(Of IList(Of String))</returns>
        /// <remarks></remarks>
        public IList<IList<string>> Patterns
        {
            get { return patternList; }
        }
        #endregion

        #region "Constructor"
        /// <summary>
        ///  Default new Device Pattern data
        /// </summary>
        /// <remarks>-</remarks>
        public Pattern()
        {
            patternList = new List<IList<string>>();
        }
        #endregion

        #region "Functions"
        /// <summary>
        ///  Returns true if one of the patterns in patternList occurs in Device Pattern data
        /// </summary>
        /// <param name="patternList">List(Of String)</param>
        /// <returns>Boolean</returns>
        /// <remarks>-</remarks>
        public bool isValid(List<string> patternList)
        {
            bool found = false;
            foreach (IList<string> patternset in Patterns)
            {
                foreach (string pattern in patternset)
                {
                    if (!patternList.Contains(pattern))
                    {
                        goto patternsContinue;
                    }
                }
                found = true;
                break;
            patternsContinue: ;
            }
            return found;
        }
        /// <summary>
        ///  To String override
        /// </summary>
        /// <returns>String</returns>
        /// <remarks>-</remarks>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (List<string> sublist in patternList)
            {
                builder.AppendFormat("'{0}',", string.Join(",", sublist.ToArray()));
            }
            return builder.ToString().TrimEnd(',');
        }
        #endregion
    }
}