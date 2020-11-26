using DIUnitTests.TestClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIUnitTests
{
    public class TImplementation : TDependency
    {
        public override bool DoNothing(IAnimal animal) { throw new NotImplementedException(); }

    }
}
