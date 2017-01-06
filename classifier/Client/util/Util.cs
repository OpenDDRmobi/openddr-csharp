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
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
/*
using log4net;
using log4net.Repository.Hierarchy;
using log4net.Core;
using log4net.Appender;
using log4net.Layout;
*/

//
namespace ClassifierClient
{
    public sealed class Util
    {
        // Private
        /// <summary>
        ///  Exception to string
        /// </summary>
        /// <param name="id">some id</param>
        /// <param name="ex">Exception</param>
        /// <returns>String</returns>
        /// <remarks>-</remarks>
        private static string ExceptionToString(string id, Exception ex)
        {
            StringBuilder result = new StringBuilder();
            if (!string.IsNullOrEmpty(id))
            {
                result.AppendLine(string.Format("Id : {0}", id));
            }
            result.AppendLine(string.Format("Message : {0}", ex.Message));
            if (ex.Data.Count != 0)
            {
                result.AppendLine("Data : ");
                foreach (DictionaryEntry de in ex.Data)
                {
                    result.AppendLine(de.Key.ToString());
                }
            }
            result.AppendLine(string.Format("Source : {0}", ex.Source));
            result.AppendLine(string.Format("TargetSite : {0}", ex.TargetSite.ToString()));
            result.AppendLine(string.Format("StackTrace : {0}", ex.StackTrace));
            return result.ToString();
        }
        // Public
        /// <summary>
        ///  Returns directory of config file of running assembly
        /// </summary>
        /// <returns>String</returns>
        /// <remarks>-</remarks>
        public static string Home()
        {
            return new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile).DirectoryName;
        }
        /// <summary>
        ///  Console debug messages
        /// </summary>
        /// <param name="msg">message</param>
        /// <remarks>-</remarks>
        public static void Log(string msg)
        {
            Log(msg, null);
        }
        /// <summary>
        ///  Console debug exception messages
        /// </summary>
        /// <param name="msg">message</param>
        /// <param name="e">Exception</param>
        /// <remarks>-</remarks>
        public static void Log(string msg, Exception e)
        {
            Console.WriteLine(string.Format("{0} - {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"), msg));
            if (e != null)
            {
                Console.WriteLine("Exception");
                Console.WriteLine(ExceptionToString("console", e));
            }
        }
        /// <summary>
        ///  Normalize pattern to letters and digits only
        /// </summary>
        /// <param name="dirty">string to normalize</param>
        /// <returns>String</returns>
        /// <remarks>-</remarks>
        public static string Normalize(string dirty)
        {
            if (string.IsNullOrEmpty(dirty))
            {
                return dirty;
            }
            dirty = dirty.ToLower().Trim().Replace("[bb]", "b");
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i <= dirty.Length - 1; i++)
            {
                System.Nullable<char> c = dirty[i];
                if (char.IsLetter(Convert.ToChar(c)) || char.IsDigit(Convert.ToChar(c)))
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }
        //  public static
        /// <summary>
        ///  Log Object to Application Log
        /// </summary>
        /// <param name="entryStr">String</param>
        /// <param name="type">EventLogEntryType</param>
        /// <remarks></remarks>
        /*       public static void WriteEntry(string appName, string entryStr, EventLogEntryType type = EventLogEntryType.Warning, Exception ex = null)
               {
                   using (EventLog myLog = new EventLog())
                   {
                       myLog.Source = appName;
                       if (!EventLog.SourceExists(myLog.Source))
                       {
                           EventLog.CreateEventSource(myLog.Source, "Application");
                       }
                       if (ex != null)
                       {
                           entryStr = string.Format("{0} : {1}", entryStr, ExceptionToString("ex", ex));
                       }
                       byte[] btText = Encoding.UTF8.GetBytes(entryStr);
                       if (btText.Length > 32766)
                       {
                           // The message string is longer than 32766 bytes.
                           entryStr = BitConverter.ToString(btText, 0, 32760);
                       }
                       myLog.WriteEntry(entryStr, type);
                   }
               } */
/*
        public static void SetupLogging()
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
            patternLayout.ActivateOptions();

            RollingFileAppender roller = new RollingFileAppender();
            roller.AppendToFile = true;
            roller.File = @"OpenDDR.log";
            roller.Layout = patternLayout;
            roller.MaxSizeRollBackups = 5;
            roller.MaximumFileSize = "500MB";
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.StaticLogFileName = true;
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            MemoryAppender memory = new MemoryAppender();
            memory.ActivateOptions();
            hierarchy.Root.AddAppender(memory);

            hierarchy.Root.Level = Level.Info;
            hierarchy.Configured = true;
        }
 */
    }
}