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
using W3c.Ddr.Models;
using W3c.Ddr.Exceptions;
using System.Reflection;

namespace W3c.Ddr.Simple
{
    /// <summary>
    /// Defines a factory for instantiating Service with the supplied default namespace and configuration.
    /// </summary>
    public class ServiceFactory
    {
        
        //public static IService newService(String clazz, String defaultVocabulary, Properties configuration)
        /// <summary>
        /// Instantiates an instance of the Type serviceType establishing the Default Vocabulary to be the one specified and with implementation specific values passed as Properties.
        /// </summary>
        /// <param name="serviceType">The interface implementation</param>
        /// <param name="defaultVocabulary">The default vocabulary</param>
        /// <param name="configuration">The Property</param>
        /// <returns>Return the service instance</returns>
        /// <exception cref="InitializationException">Throws when...</exception>
        /// <exception cref="NameException">Throws when...</exception>
        public static IService newService(Type serviceType, String defaultVocabulary, Properties configuration)
        {

		    IService theService = null;

            if (serviceType == null)
            {
                throw new W3c.Ddr.Exceptions.SystemException(W3c.Ddr.Exceptions.SystemException.ILLEGAL_ARGUMENT, "Service class cannot be null");
		    }

		    if (defaultVocabulary == null)
            {
                throw new W3c.Ddr.Exceptions.SystemException(W3c.Ddr.Exceptions.SystemException.ILLEGAL_ARGUMENT, "Default vocabulary cannot be null");
		    }

            try
            {
                // Instantiation
                //Type serviceType = Type.GetType(clazz, true);
                theService = (IService)Activator.CreateInstance(serviceType);
            }
            catch (TargetInvocationException e)
            {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, e);
            }
            catch (ArgumentException e)
            {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, e);
            }
            catch (Exception thr)
            {
                throw new W3c.Ddr.Exceptions.SystemException(W3c.Ddr.Exceptions.SystemException.CANNOT_PROCEED, thr);
            }

		    // Initialization
		    theService.Initialize(defaultVocabulary, configuration);

		    return theService;
	    }
    }
}
