using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIUnitTests.TestClasses
{
    public class Car : IVehicle
    {
        public bool DoNothing()
        {
            throw new NotImplementedException();
        }
    }
}
