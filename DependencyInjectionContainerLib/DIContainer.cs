using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainerLib
{
    public class DIContainer
    {
        DIConfiguration configuration;

        public DIContainer(DIConfiguration configuration)
        {
            validateConfiguration(configuration);
            this.configuration = configuration;
        }

        public void validateConfiguration(DIConfiguration configuration)
        {
            foreach (Type tDependency in configuration.dependenciesContainer.Keys)
            {
                if (tDependency.IsValueType)
                {
                    throw new ArgumentException("TDependency must be a reference type");
                }

                foreach (Type tImplementation in configuration.dependenciesContainer[tDependency])
                {
                    // Checks if TImplementation inherits from/implements TDependency
                    if (!tDependency.IsAssignableFrom(tImplementation) && !tDependency.IsGenericTypeDefinition)
                    {
                        throw new ArgumentException("TImplementation must be inherited from/implement Dependency type.");
                    }

                    if (tImplementation.IsAbstract || !tImplementation.IsClass)
                    {
                        throw new ArgumentException("TImplementation must be a non-abstract class");
                    }
                }
            }
        }

        public List<TDependency> Resolve<TDependency>()
        {
            Type tDependency = typeof(TDependency);
            if (tDependency.IsGenericType && tDependency.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
            {
                tDependency = tDependency.GenericTypeArguments[0];
            }

            if (tDependency.IsGenericType)
            {
                foreach (var type in tDependency.GenericTypeArguments)
                {
                    if (!configuration.dependenciesContainer.ContainsKey(type))
                    {
                        throw new KeyNotFoundException($"Dependency {type.ToString()} is not registered.");
                    }
                }
            }
            else
            if (!configuration.dependenciesContainer.ContainsKey(tDependency))
            {
                throw new KeyNotFoundException($"Dependency {tDependency.ToString()} is not registered.");
            }

            List<TDependency> result = new List<TDependency>();

            if (tDependency.IsGenericType && configuration.dependenciesContainer.ContainsKey(tDependency.GetGenericTypeDefinition()))
            {

                var implementations = configuration.dependenciesContainer[tDependency.GetGenericTypeDefinition()];
                foreach (var implementation in implementations)
                {
                    var genericType = implementation.MakeGenericType(tDependency.GetGenericArguments());
                    var resolved = (TDependency)GetInstance(genericType);
                    result.Add(resolved);
                }

                return result;

            }
            else
            if (!configuration.dependenciesContainer.ContainsKey(tDependency))
            {
                throw new KeyNotFoundException($"Dependency {tDependency.ToString()} is not registered.");
            }

            foreach (var implementation in configuration.dependenciesContainer[tDependency])
            {
                TDependency resolved;

                if (configuration.lifetimeSettings[implementation])
                {
                    if (configuration.objectContainer[implementation].instance == null)
                    {
                        lock (configuration.objectContainer[implementation].syncRoot)
                        {
                            if (configuration.objectContainer[implementation].instance == null)
                            {
                                configuration.objectContainer[implementation].instance = GetInstance(implementation);
                            }
                        }
                    }

                    resolved = (TDependency)configuration.objectContainer[implementation].instance;

                }
                else
                {
                    resolved = (TDependency)GetInstance(implementation);
                }

                result.Add(resolved);
            }

            return result;

        }

        // Resolve for creating inner dependencies using reflection.
        public object ResolveFromType(Type t)
        {
            var resolveMethod = typeof(DIContainer).GetMethod("Resolve");
            var resolveType = resolveMethod.MakeGenericMethod(t);

            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(t);

            dynamic resolved = Convert.ChangeType(resolveType.Invoke(this, null), constructedListType);

            if (resolved.Count > 1)
            {
                return resolved;
            }
            else
            {
                return resolved[0];
            }
        }

        private object GetInstance(Type t)
        {
            ConstructorInfo constructor = t.GetConstructors().OrderBy(x => x.GetParameters().Length).Last();

            if (constructor != null)
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                object[] parameterInstances = new object[parameters.Length];

                int index = 0;
                foreach (var param in parameters)
                {

                    Type paramType = param.ParameterType;

                    if (paramType.IsGenericType && paramType.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
                    {
                        paramType = paramType.GenericTypeArguments[0];
                    }

                    if (paramType.IsGenericType)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        if (configuration.dependenciesContainer.ContainsKey(paramType))
                        {
                            parameterInstances[index] = ResolveFromType(paramType);
                        }
                    }

                    index++;
                }

                var instance = constructor.Invoke(parameterInstances);
                return instance;

            }
            else
            {
                throw new InvalidOperationException($"No public constructors available for {t}");
            }

        }
    }
}
