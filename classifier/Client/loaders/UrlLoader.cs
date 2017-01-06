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
using System;
using System.IO;
using System.Net;
/// <summary>
///  Load XML resources from URL
/// </summary>
/// <remarks>-</remarks>
internal sealed class UrlLoader //: ILoader
{
    /// <summary>
    ///  Resource URI
    /// </summary>
    /// <returns>String</returns>
    /// <remarks>-</remarks>
    public string Path
    {
        get
        {
            return this.resUrl;
        }
    }

    /// <summary>
    ///  Reader
    /// </summary>
    /// <returns>StreamReader</returns>
    /// <remarks>-</remarks>
    public StreamReader Reader
    {
        get
        {
            return this.resReader;
        }
    }

    private long resLength
    {
        get;
        set;
    }

    public long ResponseLenght
    {
        get
        {
            return this.resLength;
        }
    }

    private StreamReader resReader
    {
        get;
        set;
    }

    private string resUrl
    {
        get;
        set;
    }

    public UrlLoader(string url)
    {
        this.resLength = (long)-1;
        this.resReader = null;
        this.resUrl = string.Empty;
        this.resUrl = url.Trim();
        int resStatus = 0;
        HttpWebRequest ddrRequest = (HttpWebRequest)WebRequest.Create(new Uri(this.resUrl));
        ddrRequest.UserAgent = string.Format("{0} {1}", "mobi.openddr.classifier.client", "1.0");
        ddrRequest.AllowAutoRedirect = false;
        WebResponse ddrResponse = ddrRequest.GetResponse();
        resStatus = (int)((HttpWebResponse)ddrResponse).StatusCode;
        int num = resStatus;
        if (num != 200)
        {
            if (num <= 299)
            {
                throw new ArgumentException(string.Format("Weird HTTP Status code : {0}", resStatus.ToString()));
            }
            throw new ArgumentException(string.Format("HTTP Status code : {0}", resStatus.ToString()));
        }
        this.resLength = ddrResponse.ContentLength;
        this.resReader = new StreamReader(ddrResponse.GetResponseStream());
    }
}
