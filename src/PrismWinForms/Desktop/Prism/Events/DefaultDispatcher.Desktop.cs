//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System;
using System.Threading;

namespace Microsoft.Practices.Prism.Events
{
    /// <summary>
    /// Wraps the Application Dispatcher.
    /// </summary>
    [Obsolete]
    public class DefaultDispatcher : IDispatcherFacade
    {
		internal static SynchronizationContext UISynchronizationContext;

        /// <summary>
        /// Forwards the BeginInvoke to the current application's <see cref="Dispatcher"/>.
        /// </summary>
        /// <param name="method">Method to be invoked.</param>
        /// <param name="arg">Arguments to pass to the invoked method.</param>
        public void BeginInvoke(Delegate method, object arg)
        {
			var sc = UISynchronizationContext;
			if (sc != null)
				sc.Post(o => method.DynamicInvoke(o), arg);
        }
    }
}