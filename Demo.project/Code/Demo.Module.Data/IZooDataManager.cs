using Demo.Module.Entities;
using System;
using System.Collections.Generic;

namespace Demo.Module.Data
{
    public interface IZooDataManager : IDataManager
    {
        List<Zoo> GetZoos();
        bool Insert(Zoo zoo, out long insertedId);
        bool Update(Zoo zoo);
    }
}
