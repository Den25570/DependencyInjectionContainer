using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainerLib
{
    public class DIConfiguration
    { 
    public Dictionary<Type, bool> lifetimeSettings;

    public Dictionary<Type, SingletonContainer> objectContainer;

    public Dictionary<Type, List<Type>> dependenciesContainer;


    public void Register<TDependency, TImplementation>(bool isSingleton = false)
    {
        Type tDependency = typeof(TDependency);
        Type tImplementation = typeof(TImplementation);

        if (!dependenciesContainer.ContainsKey(tDependency))
        {
            dependenciesContainer[tDependency] = new List<Type>();
            dependenciesContainer[tDependency].Add(tImplementation);
        }
        else
        {
            if (!dependenciesContainer[tDependency].Contains(tImplementation))
            {
                dependenciesContainer[tDependency].Add(tImplementation);
            }
        }

        lifetimeSettings[tImplementation] = isSingleton;

        if (isSingleton)
        {
            objectContainer[tImplementation] = new SingletonContainer();
        }
    }

    public void Register(Type tDependency, Type tImplementation)
    {
        if (!dependenciesContainer.ContainsKey(tDependency))
        {
            dependenciesContainer[tDependency] = new List<Type>();
            dependenciesContainer[tDependency].Add(tImplementation);
        }
        else
        {
            if (!dependenciesContainer[tDependency].Contains(tImplementation))
            {
                dependenciesContainer[tDependency].Add(tImplementation);
            }
        }
    }

    public DIConfiguration()
    {
        lifetimeSettings = new Dictionary<Type, bool>();
        objectContainer = new Dictionary<Type, SingletonContainer>();
        dependenciesContainer = new Dictionary<Type, List<Type>>();
    }
}
}
