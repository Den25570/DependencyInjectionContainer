using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainerLib
{
    [AttributeUsage(AttributeTargets.All)]
    public class DependencyKey : Attribute
    {
        public object Name { get; private set; }

        public DependencyKey(object name)
        {
            Name = name;
        }
    }
}
