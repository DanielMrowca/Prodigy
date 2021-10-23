using System;
using Microsoft.Extensions.DependencyInjection;

namespace Prodigy
{
    public interface IProdigyBuilder
    {
        IServiceCollection Services { get; }
        void AddBuildAction(Action<IServiceProvider> execute);
        IServiceProvider Build();
    }
}
