using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface IVolumeCommitmentDataManager:IDataManager
    {
        List<VolumeCommitment> GetVolumeCommitments();

        bool AreVolumeCommitmentsUpdated(ref object updateHandle);

        bool Insert(VolumeCommitment volumeCommitment, out int insertedId);

        bool Update(VolumeCommitment volumeCommitment);
    }
}
