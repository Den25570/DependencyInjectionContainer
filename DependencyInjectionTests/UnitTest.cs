using DependencyInjectionContainerLib;
using DIUnitTests;
using DIUnitTests.TestClasses;
using NUnit.Framework;
using System;

namespace DependencyInjectionTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        // Checks that the same TImplementations are not registered twice
        [Test]
        public void TestDuplicateImplementations()
        {
            var dependencies = new DIConfiguration();
            dependencies.Register<TDependency, TImplementation>(true);
            dependencies.Register<TDependency, TImplementation>(true);
            var provider = new DIContainer(dependencies);

            Assert.AreEqual(dependencies.dependenciesContainer[typeof(TDependency)].Count, 1);
        }

        // Simple check for resolving basic implementation.
        [Test]
        public void TestResolvingBasicImplementation()
        {
            var dependencies = new DIConfiguration();
            dependencies.Register<TDependency, TImplementation>(true);
            var provider = new DIContainer(dependencies);
            var obj = provider.Resolve<TDependency>();
            Assert.That(obj[0], Is.InstanceOf(typeof(TImplementation)));
        }

        // Checks for for resolving dependencies with multiple implementations
        [Test]
        public void TestResolvingMultipleImplementations()
        {
            var dependencies = new DIConfiguration();
            dependencies.Register<TDependency, TImplementation>(true);
            dependencies.Register<TDependency, TComplexImplementation>(true);
            var provider = new DIContainer(dependencies);
            var resolved = provider.Resolve<TDependency>();
            Assert.AreEqual(resolved.Count, 2);

            Type[] actual = new Type[] { resolved[0].GetType(), resolved[1].GetType() };
            Type[] expected = new Type[] { typeof(TComplexImplementation), typeof(TImplementation) };

            CollectionAssert.AreEquivalent(actual, expected);
        }


        [Test]
        public void TestRecursiveDependencies()
        {
            var dependencies = new DIConfiguration();
            dependencies.Register<TDependency, TComplexImplementation>(true);
            dependencies.Register<IAnimal, Dog>(true);
            dependencies.Register<IVehicle, Car>(true);
            dependencies.Register<IVehicle, Bike>(true);
            var provider = new DIContainer(dependencies);

            var complexImplementation = (TComplexImplementation)provider.Resolve<TDependency>()[0];


            // Check that IEnumerable<IVehicle> dependencies are created.
            Type[] actual = new Type[] { complexImplementation.vehicles[0].GetType(), complexImplementation.vehicles[1].GetType() };
            Type[] expected = new Type[] { typeof(Car), typeof(Bike) };

            CollectionAssert.AreEquivalent(actual, expected);

            // Check that IAnimal dependency is created.
            Assert.AreEqual(typeof(Dog), complexImplementation.animal.GetType());

        }

        // Test singleton lifestyle-parameter
        [Test]
        public void TestSingletonDependencies()
        {
            var dependencies = new DIConfiguration();
            dependencies.Register<TDependency, TImplementation>(true);
            dependencies.Register<IAnimal, Dog>(false);
            var provider = new DIContainer(dependencies);

            var firstObject = provider.Resolve<TDependency>()[0];
            var secondObject = provider.Resolve<TDependency>()[0];

            Assert.AreSame(firstObject, secondObject);

            var thirdObject = provider.Resolve<IAnimal>()[0];
            var fourthObject = provider.Resolve<IAnimal>()[0];

            Assert.AreNotSame(thirdObject, fourthObject);
        }

        // Test registering and resolving of generic dependencies
        [Test]
        public void TestGenericDependencies()
        {

            var dependencies = new DIConfiguration();
            dependencies.Register<IService<TDependency>, ServiceImpl<TDependency>>(true);
            dependencies.Register<TDependency, TImplementation>(true);
            var provider = new DIContainer(dependencies);

            var genericObject = (ServiceImpl<TDependency>)provider.Resolve<IService<TDependency>>()[0];

            Assert.AreEqual(genericObject.repository.GetType(), typeof(TImplementation));
        }

        // Test registering and resolving with open generics
        [Test]
        public void TestOpenGenericDependencies()
        {

            var dependencies = new DIConfiguration();
            dependencies.Register(typeof(IService<>), typeof(ServiceImpl<>));
            dependencies.Register<TDependency, TImplementation>(true);
            var provider = new DIContainer(dependencies);

            var genericObject = (ServiceImpl<TDependency>)provider.Resolve<IService<TDependency>>()[0];

            Assert.AreEqual(genericObject.repository.GetType(), typeof(TImplementation));
        }
    }
}