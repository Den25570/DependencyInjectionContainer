﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIUnitTests.TestClasses
{
    public interface IService<TRepository> where TRepository : TDependency
    {
        void DoWhatYouAreBestAt(TRepository repository);
    }
}
