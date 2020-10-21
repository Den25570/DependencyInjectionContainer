using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainerLib
{
    public class DIConfiguration
    { 
    // Bool value is true if object lifetime type is Singletone
    public Dictionary<Type, bool> lifetimeSettings;

    // Dictionary to story created objects for sigletone types
    public Dictionary<Type, SingletonContainer> objectContainer;

    // Dictionary to map Abstracy Dependency to Concrete Implementation(s)
    public Dictionary<Type, List<Type>> dependenciesContainer;


    public void Register<TDependency, TImplementation>(bool isSingleton = false)
    {
        Type tDependency = typeof(TDependency);
        Type tImplementation = typeof(TImplementation);

        // Register dependency in a dictionary
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

        // Register type lifetime settings
        lifetimeSettings[tImplementation] = isSingleton;

        // Register and type in the object storage
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
            // Implementations in array must be unique
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
