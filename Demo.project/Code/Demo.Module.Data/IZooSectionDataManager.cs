using Demo.Module.Entities;
using System.Collections.Generic;

namespace Demo.Module.Data
{
    public interface IZooSectionDataManager : IDataManager
    {
        List<ZooSection> GetZooSections();
        bool Insert(ZooSection zooSection, out long insertedId);
        bool Update(ZooSection zooSection);
    }
}
