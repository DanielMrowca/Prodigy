using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Prodigy.Types;

namespace Prodigy
{
    public class ProdigyBuilder : IProdigyBuilder
    {
        private readonly List<Action<IServiceProvider>> _buildActions;
        public IServiceCollection Services { get; }
       
        private ProdigyBuilder(IServiceCollection services)
        {
            _buildActions = new List<Action<IServiceProvider>>();
            Services = services;
            Services.AddSingleton<IStartupInitializer, StartupInitializer>();
        }

        public static IProdigyBuilder Create(IServiceCollection services)
            => new ProdigyBuilder(services);

        public void AddBuildAction(Action<IServiceProvider> execute)
        {
            _buildActions.Add(execute);
        }

        public IServiceProvider Build()
        {
            var serviceProvider = Services.BuildServiceProvider();
            foreach (var buildAction in _buildActions)
            {
                buildAction.Invoke(serviceProvider);
            }

            return serviceProvider;
        }
    }
}
