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
            this.configuration = configuration;
        }

        public object Resolve<TDependency>(object name = null)
        {
            Type tDependency;
            bool returnEnumerable = ExtractTypeIFEnumerable(typeof(TDependency), out tDependency);

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

            if (tDependency.IsGenericType && configuration.dependenciesContainer.ContainsKey(tDependency.GetGenericTypeDefinition()))
            {

                var implementations = configuration.dependenciesContainer[tDependency.GetGenericTypeDefinition()];

                if (returnEnumerable)
                {
                    List<object> result = new List<object>();
                    foreach (var implementation in implementations)
                        result.Add(GetGenericTypeImplementation<TDependency>(tDependency, implementation));
                    return result;
                }
                else
                {                 
                    return GetGenericTypeImplementation<TDependency>(tDependency, implementations.First());
                }

            }
            else
            if (!configuration.dependenciesContainer.ContainsKey(tDependency))
            {
                throw new KeyNotFoundException($"Dependency {tDependency.ToString()} is not registered.");
            }

            if (returnEnumerable)
            {
                List<object> result = new List<object>();
                foreach (var implementation in configuration.dependenciesContainer[tDependency].
                Where(implementation => name == null || implementation.Name.Equals(name)))
                {
                    result.Add(GetImplementation(implementation));
                }
                return result;
            }
            else
            {
                var implementations = configuration.dependenciesContainer[tDependency].
                    Where(dImplementation => (name == null) || (dImplementation.Name.Equals(name)));

                return GetImplementation(implementations.First());

            }        
        }

        private object GetImplementation(DependencyImplementation implementation)
        {
            object resolved;

            if (configuration.lifetimeSettings[implementation.Implementation])
            {
                if (configuration.objectContainer[implementation.Implementation].instance == null)
                {
                    lock (configuration.objectContainer[implementation.Implementation].syncRoot)
                    {
                        if (configuration.objectContainer[implementation.Implementation].instance == null)
                        {
                            configuration.objectContainer[implementation.Implementation].instance = GetInstance(implementation.Implementation);
                        }
                    }
                }

                resolved = configuration.objectContainer[implementation.Implementation].instance;

            }
            else
            {
                resolved = GetInstance(implementation.Implementation);
            }

            return resolved;
        }

        private TDependency GetGenericTypeImplementation<TDependency>(Type tDependency, DependencyImplementation implementation)
        {
            var genericType = implementation.Implementation.MakeGenericType(tDependency.GetGenericArguments());
            var resolved = (TDependency)GetInstance(genericType);
            return resolved;
        }

        private bool ExtractTypeIFEnumerable(Type originalType, out Type tDependency)
        {
            tDependency = originalType;
            if ((tDependency.IsGenericType && tDependency.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>))) ||
                tDependency.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                tDependency = tDependency.GenericTypeArguments[0];
                return true;
            }

            return false;
        }

        private object ConvertList(List<object> items, Type type, bool performConversion = false)
        {
            var containedType = type.GenericTypeArguments.First();
            var enumerableType = typeof(System.Linq.Enumerable);
            var castMethod = enumerableType.GetMethod(nameof(System.Linq.Enumerable.Cast)).MakeGenericMethod(containedType);
            var toListMethod = enumerableType.GetMethod(nameof(System.Linq.Enumerable.ToList)).MakeGenericMethod(containedType);

            IEnumerable<object> itemsToCast = items;

            var castedItems = castMethod.Invoke(null, new[] { itemsToCast });

            return toListMethod.Invoke(null, new[] { castedItems });
        }

        // Resolve for creating inner dependencies using reflection.
        public object ResolveFromType(Type t)
        {
            var resolveMethod = typeof(DIContainer).GetMethod("Resolve");
            var resolveType = resolveMethod.MakeGenericMethod(t);

            object resolved = resolveType.Invoke(this, new object[1] { null });
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
            {               
                var listType = typeof(List<>);
                var constructedListType = listType.MakeGenericType(t.GetGenericArguments()[0]);
                resolved = ConvertList(resolved as List<object>, constructedListType);
            }


            return resolved;
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

                    parameterInstances[index] = ResolveFromType(paramType);

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
