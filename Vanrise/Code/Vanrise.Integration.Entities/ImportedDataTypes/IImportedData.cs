using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public interface IImportedData
    {
        string Description { get; }

        long? BatchSize { get; }

        void OnDisposed();
    }
}
