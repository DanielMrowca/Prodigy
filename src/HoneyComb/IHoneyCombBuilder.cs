using HoneyComb.Types;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HoneyComb
{
    public interface IHoneyCombBuilder
    {
        IServiceCollection Services { get; }

        void AddInitializer(IInitializer initializer);
        void AddInitializer<TInitializer>() where TInitializer : IInitializer;
        void AddBuildAction(Action<IServiceProvider> execute);
        IServiceProvider Build();
    }
}
