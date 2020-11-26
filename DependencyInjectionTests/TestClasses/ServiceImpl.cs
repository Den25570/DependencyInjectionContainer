using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIUnitTests.TestClasses
{
    class ServiceImpl<TRepository> : IService<TRepository> where TRepository : TDependency
    {

        public TRepository repository;

        public void DoWhatYouAreBestAt(TRepository repository)
        {
            repository.DoNothing(null);
        }

        public ServiceImpl(TRepository repository)
        {
            this.repository = repository;
        }

    }
}
