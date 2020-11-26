using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIUnitTests.TestClasses
{
    public class TComplexImplementation : TDependency
    {

        public IAnimal animal;
        public List<IVehicle> vehicles;

        public override bool DoNothing(IAnimal animal){ return animal.DoNothing(); }

        public TComplexImplementation(IAnimal animal, IEnumerable<IVehicle> vehicles)
        {
            this.animal = animal;
            this.vehicles = (List<IVehicle>) vehicles;
        }

    }
}
