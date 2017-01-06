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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ClassifierClient;
//using log4net;
class Program
{
    //Here is the once-per-class call to initialize the log object
    //private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    static void Main(string[] args)
    {
        //Util.SetupLogging();
        string testDataDirectory = Util.Home();
        string testDataFile = @"ua_strings.txt";
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        Classifier client = null;
        Console.WriteLine("Loading...");
        try
        {
            client = new Classifier();
        }
        catch (OpenDDRException oddrException)
        {
            //Util.WriteEntry("Classifier", oddrException.Message, EventLogEntryType.Error, oddrException);
            //log.Fatal("Client Error", oddrException);
            //Console.WriteLine(oddrException.Message);
            Util.Log("Client Error", oddrException);
        }
        stopWatch.Stop();
        Console.Clear();
        if (client != null)
        {
            Console.WriteLine("Loaded !");
            Console.WriteLine(string.Format("Classifier Client : {0}", client.Version));
            string str = client.DeviceCount.ToString();
            string str1 = client.PatternCount.ToString();
            double totalMilliseconds = stopWatch.Elapsed.TotalMilliseconds;
            Console.WriteLine(string.Format("Loaded {0} devices with {1} patterns in {2} ms", str, str1, totalMilliseconds.ToString()));
            stopWatch.Restart();
            Console.WriteLine("Cold run");
            Map(client, "Mozilla/5.0 (Linux; U; Android 2.2; en; HTC Aria A6380 Build/ERE27) AppleWebKit/540.13+ (KHTML, like Gecko) Version/3.1 Mobile Safari/524.15.0");
            Map(client, "Mozilla/5.0 (iPad; U; CPU OS 4_3_5 like Mac OS X; en-us) AppleWebKit/533.17.9 (KHTML, like Gecko) Mobile/8L1");
            Map(client, "Mozilla/5.0 (BlackBerry; U; BlackBerry 9810; en-US) AppleWebKit/534.11+ (KHTML, like Gecko) Version/7.0.0.261 Mobile Safari/534.11+");
            Map(client, "Mozilla/5.0 (iPhone; CPU iPhone OS 6_0 like Mac OS X; en-us) AppleWebKit/536.26 (KHTML, like Gecko) CriOS/23.0.1271.91 Mobile/10A403 Safari/8536.25");
            stopWatch.Stop();
            totalMilliseconds = stopWatch.Elapsed.TotalMilliseconds;
            Console.WriteLine(string.Format("End cold run : {0} ms", totalMilliseconds.ToString()));
            char[] chrArray = new char[] { '\\' };
            string data = string.Format("{0}\\{1}", testDataDirectory.TrimEnd(chrArray), testDataFile);
            Console.WriteLine(string.Format("Press any key to run test file : {0}", data));
            Console.ReadKey();
            if (File.Exists(data))
            {
                List<string> lines = new List<string>(File.ReadAllLines(data).ToList());
                List<string> cleanLines = (from l in lines select l into l where !string.IsNullOrWhiteSpace(l) select l).ToList<string>();
                stopWatch.Restart();
                int i = 0;
                foreach (string ua in cleanLines)
                {
                    Map(client, ua.Trim());
                    i = i + 1;
                }
                stopWatch.Stop();
                Console.WriteLine(string.Format("Tested {0} User-Agent strings in {1} ms.", i.ToString(), stopWatch.Elapsed.TotalMilliseconds.ToString()));
            }
            else
            {
                Console.WriteLine(string.Format("Test file {0} not found.", data));
            }
            Console.WriteLine("Press any key to finish");
            Console.ReadKey();
        } else
        {
            //log.Error("Client or Data not found");
            Console.WriteLine("Classifier Client or Data not found.");
            Console.WriteLine();
            Console.WriteLine("You may need to check your configuration '" + Process.GetCurrentProcess().ProcessName + ".exe.config'.");
        }
    }

    public static void Map(Classifier client, string text)
    {
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        IDictionary<string, string> ret = client.Map(text);
        stopWatch.Stop();
        string deviceId = "unknown";
        if (ret != null)
        {
            deviceId = ret["id"];
        }
        Console.WriteLine("Result: " + deviceId + " took " + stopWatch.Elapsed.TotalMilliseconds.ToString() + " ms");
    }
}