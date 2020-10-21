using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainerLib
{
    public class SingletonContainer
    {
        public volatile object instance;
        public object syncRoot = new object();

        public SingletonContainer()
        {
            instance = null;
        }
    }
}
