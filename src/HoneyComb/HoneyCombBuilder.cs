using HoneyComb.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace HoneyComb
{
    public class HoneyCombBuilder : IHoneyCombBuilder
    {
        private readonly List<Action<IServiceProvider>> _buildActions;
        public IServiceCollection Services { get; }
       

        private HoneyCombBuilder(IServiceCollection services)
        {
            _buildActions = new List<Action<IServiceProvider>>();
            Services = services;
            Services.AddSingleton<IStartupInitializer>(new StartupInitializer());
        }

        public static IHoneyCombBuilder Create(IServiceCollection services)
            => new HoneyCombBuilder(services);

        public void AddInitializer(IInitializer initializer)
        {
            AddBuildAction(sp =>
            {
                var startupInitializer = sp.GetRequiredService<IStartupInitializer>();
                startupInitializer.AddInitializer(initializer);
            });
        }

        public void AddInitializer<TInitializer>() where TInitializer : IInitializer
        {
            AddBuildAction(sp =>
            {
                var initializer = sp.GetRequiredService<TInitializer>();
                var startupInitializer = sp.GetRequiredService<IStartupInitializer>();
                startupInitializer.AddInitializer(initializer);
            });
        }

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
