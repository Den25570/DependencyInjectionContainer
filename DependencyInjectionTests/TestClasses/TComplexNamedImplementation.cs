using DependencyInjectionContainerLib;
using DIUnitTests;
using DIUnitTests.TestClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInjectionTests.TestClasses
{
    public class TComplexNamedImplementation : TDependency
    {
        public IAnimal animal;
        public List<IVehicle> vehicles;

        public override bool DoNothing(IAnimal animal) { return animal.DoNothing(); }

        public TComplexNamedImplementation(IAnimal animal, [DependencyKey(ImplementationName.First)] IEnumerable<IVehicle> vehicles)
        {
            this.animal = animal;
            this.vehicles = (List<IVehicle>)vehicles;
        }
    }
}
