using DependencyInjectionContainerLib;
using DependencyInjectionTests.TestClasses;
using DIUnitTests;
using DIUnitTests.TestClasses;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Moq;

namespace DependencyInjectionTests
{
    public class Tests
    {
        private Logger logger;

        [SetUp]
        public void Setup()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        // Checks that the same TImplementations are not registered twice
        [Test]
        public void TestDuplicateImplementations()
        {
            logger.Trace("Entering TestDuplicateImplementations Test");
            var dependencies = new DIConfiguration();
            dependencies.Register<TDependency, TImplementation>(true);
            dependencies.Register<TDependency, TImplementation>(true);
            var provider = new DIContainer(dependencies);
            logger.Trace("Container created");

            try
            {
                Assert.AreEqual(1, dependencies.dependenciesContainer[typeof(TDependency)].Count);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }
        }

        // Simple check for resolving basic implementation.
        [Test]
        public void TestResolvingBasicImplementation()
        {
            logger.Trace("Entering TestResolvingBasicImplementation Test");
            var dependencies = new DIConfiguration();
            dependencies.Register<TDependency, TImplementation>(true);
            var provider = new DIContainer(dependencies);
            logger.Trace("Container created");

            var obj = provider.Resolve<TDependency>();
            logger.Trace("Received resolve object");

            try
            {
                Assert.That(obj, Is.InstanceOf(typeof(TImplementation)));
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }
        }

        // Checks for for resolving dependencies with multiple implementations
        [Test]
        public void TestResolvingMultipleImplementations()
        {
            logger.Trace("Entering TestResolvingMultipleImplementations Test");
            var dependencies = new DIConfiguration();
            dependencies.Register<IVehicle, Car>(true);
            dependencies.Register<IVehicle, Bike>(true);
            var provider = new DIContainer(dependencies);
            logger.Trace("Container created");

            var resolved = provider.Resolve<IEnumerable<IVehicle>>() as List<object>;
            logger.Trace("Received resolve object");


            try
            {
                Assert.AreEqual(resolved.Count, 2);

                Type[] actual = new Type[] { resolved[0].GetType(), resolved[1].GetType() };
                Type[] expected = new Type[] { typeof(Car), typeof(Bike) };

                CollectionAssert.AreEquivalent(actual, expected);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }
        }


        [Test]
        public void TestRecursiveDependencies()
        {
            logger.Trace("Entering TestRecursiveDependencies Test");
            var dependencies = new DIConfiguration();
            dependencies.Register<TDependency, TComplexImplementation>(true);
            dependencies.Register<IAnimal, Dog>(true);
            dependencies.Register<IVehicle, Car>(true);
            dependencies.Register<IVehicle, Bike>(true);
            var provider = new DIContainer(dependencies);
            logger.Trace("Container created");

            var complexImplementation = (TComplexImplementation)provider.Resolve<TDependency>();
            logger.Trace("Received resolve object");

            try
            {
                // Check that IEnumerable<IVehicle> dependencies are created.
                Type[] actual = new Type[] { complexImplementation.vehicles[0].GetType(), complexImplementation.vehicles[1].GetType() };
                Type[] expected = new Type[] { typeof(Car), typeof(Bike) };

                CollectionAssert.AreEquivalent(actual, expected);

                // Check that IAnimal dependency is created.
                Assert.AreEqual(typeof(Dog), complexImplementation.animal.GetType());
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }

            logger.Trace("Testing created Object");
            Mock<Dog> dependencyMock = new Mock<Dog>();
            dependencyMock.Setup(x => x.DoNothing()).Returns(true);
            (complexImplementation as TComplexImplementation).DoNothing(dependencyMock.Object);
        }

        // Test singleton lifestyle-parameter
        [Test]
        public void TestSingletonDependencies()
        {
            logger.Trace("Entering TestSingletonDependencies Test");
            var dependencies = new DIConfiguration();
            dependencies.Register<TDependency, TImplementation>(true);
            dependencies.Register<IAnimal, Dog>(false);
            var provider = new DIContainer(dependencies);
            logger.Trace("Container created");

            var firstObject = provider.Resolve<TDependency>();
            logger.Trace("Received resolve object 1");
            var secondObject = provider.Resolve<TDependency>();
            logger.Trace("Received resolve object 2");

            try
            {
                Assert.AreSame(firstObject, secondObject);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }

            var thirdObject = provider.Resolve<IAnimal>();
            logger.Trace("Received resolve object 3");
            var fourthObject = provider.Resolve<IAnimal>();
            logger.Trace("Received resolve object 4");
          
            try
            {
                Assert.AreNotSame(thirdObject, fourthObject);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }
        }

        // Test registering and resolving of generic dependencies
        [Test]
        public void TestGenericDependencies()
        {
            logger.Trace("Entering TestGenericDependencies Test");
            var dependencies = new DIConfiguration();
            dependencies.Register<IService<TDependency>, ServiceImpl<TDependency>>(true);
            dependencies.Register<TDependency, TImplementation>(true);
            var provider = new DIContainer(dependencies);
            logger.Trace("Container created");

            var genericObject = (ServiceImpl<TDependency>)provider.Resolve<IService<TDependency>>();
            logger.Trace("Received resolve object");
      
            try
            {
                Assert.AreEqual(genericObject.repository.GetType(), typeof(TImplementation));
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }
        }

        // Test registering and resolving with open generics
        [Test]
        public void TestOpenGenericDependencies()
        {
            logger.Trace("Entering TestOpenGenericDependencies Test");
            var dependencies = new DIConfiguration();
            dependencies.Register(typeof(IService<>), typeof(ServiceImpl<>));
            dependencies.Register<TDependency, TImplementation>(true);
            var provider = new DIContainer(dependencies);
            logger.Trace("Container created");

            var genericObject = (ServiceImpl<TDependency>)provider.Resolve<IService<TDependency>>();
            logger.Trace("Received resolve object");
         
            try
            {
                Assert.AreEqual(genericObject.repository.GetType(), typeof(TImplementation));
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }
        }

        [Test]
        public void TestNamedImplementation()
        {
            logger.Trace("Entering TestNamedImplementation Test");
            var dependencies = new DIConfiguration();
            dependencies.Register<IVehicle, Car>(true, ImplementationName.First);
            dependencies.Register<IVehicle, Bike>(true, ImplementationName.Second);

            var provider = new DIContainer(dependencies);
            logger.Trace("Container created");

            var objCar = provider.Resolve<IVehicle>(ImplementationName.First);
            logger.Trace("Received resolve object 1");
            var objBike = provider.Resolve<IVehicle>(ImplementationName.Second);
            logger.Trace("Received resolve object 2");
            
            try
            {
                Assert.That(objCar, Is.InstanceOf(typeof(Car)));
                Assert.That(objBike, Is.InstanceOf(typeof(Bike)));
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }
        }

        [Test]
        public void TestComplexNamedDependency()
        {
            logger.Trace("Entering TestComplexNamedDependency Test");
            var dependencies = new DIConfiguration();
            dependencies.Register<TDependency, TComplexNamedImplementation>(true);
            dependencies.Register<IAnimal, Dog>(true);
            dependencies.Register<IVehicle, Car>(true, ImplementationName.First);
            dependencies.Register<IVehicle, Bike>(true, ImplementationName.Second);
            var provider = new DIContainer(dependencies);
            logger.Trace("Container created");

            var complexImplementation = (TComplexNamedImplementation)provider.Resolve<TDependency>();
            logger.Trace("Received resolve object");

            try
            {
                Assert.AreEqual(1, complexImplementation.vehicles.Count);
                Assert.That(complexImplementation.vehicles[0], Is.InstanceOf<Car>());
                Assert.AreEqual(typeof(Dog), complexImplementation.animal.GetType());
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }
        }

        [Test]
        public void TestExceptionSituations()
        {
            logger.Trace("Entering TestExceptionSituations Test");
            var dependencies = new DIConfiguration();

            try
            {
                logger.Trace("Trying register dependency");
                Assert.Throws<ArgumentException>(delegate { dependencies.Register<TDependency, TDependency>(true); });
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }

            var provider = new DIContainer(dependencies);
            logger.Trace("Container created");

            try
            {
                logger.Trace("Trying receive resolve object");
                Assert.Throws <KeyNotFoundException> (delegate { var genericObject = (ServiceImpl<TDependency>)provider.Resolve<IService<TDependency>>(); });               
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }

        }
    }
}