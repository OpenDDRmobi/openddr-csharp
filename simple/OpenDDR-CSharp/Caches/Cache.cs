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
using System.Runtime.CompilerServices;

namespace Oddr.Caches
{
    public class Cache : ICache
    {
        private Dictionary<string, object> dic;
        private Queue<String> queue;
        private int cacheSize;

        public Cache(int cacheSize)
        {
            Init(cacheSize);
        }

        public Cache()
        {
            Init(100);
        }

        private void Init(int cacheSize)
        {
            this.cacheSize = cacheSize;
            this.dic = new Dictionary<string, object>(cacheSize);
            this.queue = new Queue<string>(cacheSize);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public object GetCachedElement(string id)
        {
            object toRet = null;
            dic.TryGetValue(id, out toRet);
            return toRet; 
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetCachedElement(string id, object value)
        {
            dic.Add(id, value);
            queue.Enqueue(id);
            if (dic.Count > cacheSize)
            {
                String toRemove = queue.Dequeue();
                dic.Remove(toRemove);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Clear()
        {
            dic.Clear();
            queue.Clear();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int UsedEntries()
        {
            return dic.Count;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<KeyValuePair<string, object>> GetAll()
        {
            List<KeyValuePair<string, object>> list = new List<KeyValuePair<string,object>>();

            foreach (KeyValuePair<string, object> kvp in dic)
            {
                list.Add(kvp);
            }

            return list;
        }
    }
}
