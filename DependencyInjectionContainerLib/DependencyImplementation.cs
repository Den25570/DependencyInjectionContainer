using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainerLib
{
    public class DependencyImplementation
    {
        public Type Implementation;
        public object Name;

        public DependencyImplementation(Type implementation, object name = null)
        {
            Implementation = implementation;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            var item = obj as DependencyImplementation;

            return item != null && this.Implementation.Equals(item.Implementation) && this.Name.Equals(item.Name);
        }

        public override int GetHashCode()
        {
            int hashCode = 2011904165;
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(Implementation);
            hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(Name);
            return hashCode;
        }
    }
}
