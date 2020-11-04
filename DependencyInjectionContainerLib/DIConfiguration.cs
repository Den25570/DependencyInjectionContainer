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

        public Dictionary<Type, List<DependencyImplementation>> dependenciesContainer;


        public void Register<TDependency, TImplementation>(bool isSingleton = false, object implementationName = null)
        {
            Type tDependency = typeof(TDependency);
            Type tImplementation = typeof(TImplementation);

            if (tDependency.IsValueType)
            {
                throw new ArgumentException("TDependency must be a reference type");
            }
            if (!tDependency.IsAssignableFrom(tImplementation) && !tDependency.IsGenericTypeDefinition)
            {
                throw new ArgumentException("TImplementation must be inherited from/implement Dependency type.");
            }
            if (tImplementation.IsAbstract || !tImplementation.IsClass)
            {
                throw new ArgumentException("TImplementation must be a non-abstract class");
            }

            if (!dependenciesContainer.ContainsKey(tDependency))
            {
                dependenciesContainer[tDependency] = new List<DependencyImplementation>();
                dependenciesContainer[tDependency].Add(new DependencyImplementation(tImplementation, implementationName));
            }
            else
            {
                if (!dependenciesContainer[tDependency].Exists(implementation => implementation.Implementation == tImplementation))
                {
                    dependenciesContainer[tDependency].Add(new DependencyImplementation(tImplementation, implementationName));
                }
            }

            lifetimeSettings[tImplementation] = isSingleton;
            if (isSingleton)
            {
                objectContainer[tImplementation] = new SingletonContainer();
            }
        }

        public void Register(Type tDependency, Type tImplementation, object implementationName = null)
        {
            if (tDependency.IsValueType)
            {
                throw new ArgumentException("TDependency must be a reference type");
            }
            if (!tDependency.IsAssignableFrom(tImplementation) && !tDependency.IsGenericTypeDefinition)
            {
                throw new ArgumentException("TImplementation must be inherited from/implement Dependency type.");
            }
            if (tImplementation.IsAbstract || !tImplementation.IsClass)
            {
                throw new ArgumentException("TImplementation must be a non-abstract class");
            }

            if (!dependenciesContainer.ContainsKey(tDependency))
            {
                dependenciesContainer[tDependency] = new List<DependencyImplementation>();
                dependenciesContainer[tDependency].Add(new DependencyImplementation(tImplementation, implementationName));
            }
            else
            {
                if (!dependenciesContainer[tDependency].Contains(new DependencyImplementation(tImplementation)))
                {
                    dependenciesContainer[tDependency].Add(new DependencyImplementation(tImplementation, implementationName));
                }
            }
        }

        public DIConfiguration()
        {
            lifetimeSettings = new Dictionary<Type, bool>();
            objectContainer = new Dictionary<Type, SingletonContainer>();
            dependenciesContainer = new Dictionary<Type, List<DependencyImplementation>>();
        }
    }
}
