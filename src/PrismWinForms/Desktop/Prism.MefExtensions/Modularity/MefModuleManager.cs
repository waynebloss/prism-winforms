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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;

namespace Microsoft.Practices.Prism.MefExtensions.Modularity
{
    /// <summary>    
    /// Component responsible for coordinating the modules' type loading and module initialization process. 
    /// </summary>
    /// <remarks>
    /// This allows the MefBootstrapper to provide this class as a default implementation.
    /// If another implementation is found, this export will not be used.
    /// </remarks>
    [Export(typeof(IModuleManager))]
    public partial class MefModuleManager : ModuleManager, IPartImportsSatisfiedNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MefModuleManager"/> class.
        /// </summary>
        /// <param name="moduleInitializer">Service used for initialization of modules.</param>
        /// <param name="moduleCatalog">Catalog that enumerates the modules to be loaded and initialized.</param>
        /// <param name="loggerFacade">Logger used during the load and initialization of modules.</param>
        [ImportingConstructor()]
        public MefModuleManager(
            IModuleInitializer moduleInitializer,
            IModuleCatalog moduleCatalog,
            ILoggerFacade loggerFacade)
            : base(moduleInitializer, moduleCatalog, loggerFacade)
        {
        }

#if SILVERLIGHT
    /// <summary>
    /// Gets or sets the modules to be imported.
    /// </summary>
    /// <remarks>Import the available modules from the MEF container.
    /// Due to Silverlight/MEF restrictions this must be public.
    /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures"), ImportMany(AllowRecomposition = true)]
        public IEnumerable<Lazy<IModule, IModuleExport>> ImportedModules { get; set; }
#else
        /// <summary>
        /// Gets or sets the modules to be imported.
        /// </summary>
        /// <remarks>Import the available modules from the MEF container</remarks>
        [ImportMany(AllowRecomposition = true)]
        protected IEnumerable<Lazy<IModule, IModuleExport>> ImportedModules { get; set; }
#endif

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        /// <remarks>
        /// Whenever the MEF container loads new types that cause ImportedModules to be recomposed, this is called.
        /// This method ensures that as the MEF container discovered new modules, the ModuleCatalog is updated.
        /// </remarks>
        public virtual void OnImportsSatisfied()
        {
            // To prevent a double foreach loop, we key on the module type for anything already in the catalog.
            IDictionary<string, ModuleInfo> registeredModules = this.ModuleCatalog.Modules.ToDictionary(m => m.ModuleName);

            foreach (Lazy<IModule, IModuleExport> lazyModule in this.ImportedModules)
            {
                // It is important that the Metadata.ModuleType is used here. 
                // Using GetType().Name would cause the Module to be constructed here rather than lazily when the module is needed.
                Type moduleType = lazyModule.Metadata.ModuleType;

                ModuleInfo registeredModule = null;
                if (!registeredModules.TryGetValue(lazyModule.Metadata.ModuleName, out registeredModule))
                {
                    // If the module is not already in the catalog is it added.
                    ModuleInfo moduleInfo = new ModuleInfo()
                    {
                        ModuleName = lazyModule.Metadata.ModuleName,
                        ModuleType = moduleType.AssemblyQualifiedName,
                        InitializationMode = lazyModule.Metadata.InitializationMode,
                        State = lazyModule.Metadata.InitializationMode == InitializationMode.OnDemand ? ModuleState.NotStarted : ModuleState.ReadyForInitialization,
                    };

                    if (lazyModule.Metadata.DependsOnModuleNames != null)
                    {
                        moduleInfo.DependsOn.AddRange(lazyModule.Metadata.DependsOnModuleNames);
                    }

                    this.ModuleCatalog.AddModule(moduleInfo);
                }
                else
                {
                    // If the module is already in the catalog then override the module type name from the imported module
                    registeredModule.ModuleType = moduleType.AssemblyQualifiedName;
                }
            }

            this.LoadModulesThatAreReadyForLoad();
        }

        /// <summary>
        /// Checks if the module needs to be retrieved before it's initialized.
        /// </summary>
        /// <param name="moduleInfo">Module that is being checked if needs retrieval.</param>
        /// <returns>True if the module needs to be retrieved.  Otherwise, false.</returns>
        protected override bool ModuleNeedsRetrieval(ModuleInfo moduleInfo)
        {
            return this.ImportedModules == null
                || !this.ImportedModules.Any(lazyModule => lazyModule.Metadata.ModuleName == moduleInfo.ModuleName);
        }
    }
}