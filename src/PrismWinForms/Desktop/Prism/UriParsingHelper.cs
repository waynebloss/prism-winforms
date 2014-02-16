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
#if REGIONS
using Microsoft.Practices.Prism.Regions;
#endif

namespace Microsoft.Practices.Prism
{
    /// <summary>
    /// Helper class for parsing <see cref="Uri"/> instances.
    /// </summary>
    public static class UriParsingHelper
    {
        /// <summary>
        /// Gets the query part of <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri.</param>
        public static string GetQuery(Uri uri)
        {
            return EnsureAbsolute(uri).Query;
        }

        /// <summary>
        /// Gets the AbsolutePath part of <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri.</param>
        public static string GetAbsolutePath(Uri uri)
        {
            return EnsureAbsolute(uri).AbsolutePath;
        }
#if REGIONS
		/// <summary>
        /// Parses the query of <paramref name="uri"/> into a dictionary.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public static NavigationParameters ParseQuery(Uri uri)
        {
            var query = GetQuery(uri);

            return new NavigationParameters(query);
        }
#endif
        private static Uri EnsureAbsolute(Uri uri)
        {
            if (uri.IsAbsoluteUri)
            {
                return uri;
            }

            if ((uri != null) && !uri.OriginalString.StartsWith("/", StringComparison.Ordinal))
            {
                return new Uri("http://localhost/" + uri, UriKind.Absolute);
            }
            return new Uri("http://localhost" + uri, UriKind.Absolute);
        }
    }
}