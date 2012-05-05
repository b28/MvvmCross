#region Copyright
// <copyright file="MvxBaseWindowsPhoneSetup.cs" company="Cirrious">
// (c) Copyright Cirrious. http://www.cirrious.com
// This source is subject to the Microsoft Public License (Ms-PL)
// Please see license.txt on http://opensource.org/licenses/ms-pl.html
// All other rights reserved.
// </copyright>
// 
// Project Lead - Stuart Lodge, Cirrious. http://www.cirrious.com
#endregion

using System;
using System.Collections.Generic;
using Cirrious.MvvmCross.Exceptions;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.Platform.Diagnostics;
using Cirrious.MvvmCross.Interfaces.Platform.Lifetime;
using Cirrious.MvvmCross.Interfaces.Plugins;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using Cirrious.MvvmCross.Interfaces.Views;
using Cirrious.MvvmCross.Platform;
using Cirrious.MvvmCross.Views;
using Cirrious.MvvmCross.WindowsPhone.Interfaces;
using Cirrious.MvvmCross.WindowsPhone.Platform.Lifetime;
using Cirrious.MvvmCross.WindowsPhone.Views;
using Microsoft.Phone.Controls;

namespace Cirrious.MvvmCross.WindowsPhone.Platform
{
    public abstract class MvxBaseWindowsPhoneSetup 
        : MvxBaseSetup        
        , IMvxServiceProducer<IMvxWindowsPhoneViewModelRequestTranslator>
        , IMvxServiceProducer<IMvxLifetime>
        , IMvxServiceProducer<IMvxTrace>
    {
        private const string PluginPostfix = ".WindowsPhone";
        private readonly PhoneApplicationFrame _rootFrame;

        protected MvxBaseWindowsPhoneSetup(PhoneApplicationFrame rootFrame)
        {
            _rootFrame = rootFrame;
        }

        protected override void InitializeDebugServices()
        {
            this.RegisterServiceInstance<IMvxTrace>(new MvxDebugTrace());
            base.InitializeDebugServices();
        }

        protected override MvxViewsContainer CreateViewsContainer()
        {
            var container = CreateViewsContainer(_rootFrame);
            this.RegisterServiceInstance<IMvxWindowsPhoneViewModelRequestTranslator>(container);
            return container;
        }

        protected override IMvxViewDispatcherProvider CreateViewDispatcherProvider()
        {
            return CreateViewDispatcherProvider(_rootFrame);
        }

        protected virtual IMvxViewDispatcherProvider CreateViewDispatcherProvider(PhoneApplicationFrame rootFrame)
        {
            return new MvxPhoneViewDispatcherProvider(rootFrame);
        }

        protected virtual MvxPhoneViewsContainer CreateViewsContainer(PhoneApplicationFrame rootFrame)
        {
            return new MvxPhoneViewsContainer(rootFrame);
        }

        protected override IDictionary<Type, Type> GetViewModelViewLookup()
        {
            return GetViewModelViewLookup(GetType().Assembly, typeof(IMvxWindowsPhoneView));
        }

        protected override IMvxPluginManager CreatePluginManager()
        {
            var toReturn = new MvxWindowsPhonePluginManager();
            AddPluginsLoaders(toReturn.Loaders);
            return toReturn;
        }

        protected virtual void AddPluginsLoaders(Dictionary<string, Func<IMvxPlugin>> loaders)
        {
            // none added by default
        }

        protected static void AddConventionalPlugin<TPlugin>(Dictionary<string, Func<IMvxPlugin>> loaders)
            where TPlugin : IMvxPlugin
        {
            AddConventionalPlugin(loaders, typeof (TPlugin));
        }

        protected static void AddConventionalPlugin(Dictionary<string, Func<IMvxPlugin>> loaders, Type plugin)
        {
            var name = plugin.Namespace ?? string.Empty;
            if (!name.EndsWith(PluginPostfix))
            {
                throw new MvxException("You must pass in the type of a plugin instance - like 'typeof(Cirrious.MvvmCross.Plugins.Visibility.WindowsPhone.Plugin)'");
            }
            
            name = name.Substring(0, name.Length - PluginPostfix.Length);
            
            loaders.Add(
                name,
                () => (IMvxPlugin)Activator.CreateInstance(plugin));
        }


        protected override void InitializePlatformServices()
        {
            this.RegisterServiceInstance<IMvxLifetime>(new MvxWindowsPhoneLifetimeMonitor());
        }
    }
}