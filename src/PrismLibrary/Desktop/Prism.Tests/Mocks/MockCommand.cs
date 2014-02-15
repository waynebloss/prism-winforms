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
using System.Windows.Input;

namespace Microsoft.Practices.Prism.Tests.Mocks
{
    internal class MockCommand : ICommand
    {
        public bool ExecuteCalled { get; set; }
        public bool CanExecuteReturnValue = true;
        public object ExecuteParameter;
        public object CanExecuteParameter;
        public int CanExecuteTimesCalled;

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            ExecuteCalled = true;
            ExecuteParameter = parameter;
        }

        public bool CanExecute(object parameter)
        {
            CanExecuteTimesCalled++;
            CanExecuteParameter = parameter;
            return CanExecuteReturnValue;
        }

        public void RaiseCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
                this.CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}