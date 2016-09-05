using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class VolumeCommitmentDataManager:BaseSQLDataManager,IVolumeCommitmentDataManager
    {
       
        #region Constructors

        public VolumeCommitmentDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneV2DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public List<VolumeCommitment> GetVolumeCommitments()
        {
            return GetItemsSP("TOneWhS_BE.sp_VolumeCommitment_GetAll", VolumeCommitmentMapper);
        }

        public bool AreVolumeCommitmentsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.VolumeCommitment", ref updateHandle);
        }

        public bool Insert(VolumeCommitment volumeCommitment, out int insertedId)
        {
            object volumeCommitmentId;

            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_VolumeCommitment_Insert]", out volumeCommitmentId, Vanrise.Common.Serializer.Serialize(volumeCommitment.Settings));
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (int)volumeCommitmentId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }

        public bool Update(VolumeCommitment volumeCommitment)
        {
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_VolumeCommitment_Update]", volumeCommitment.VolumeCommitmentId, Vanrise.Common.Serializer.Serialize(volumeCommitment.Settings));
            return (recordsEffected > 0);
        }

        #endregion

        #region  Mappers

        private VolumeCommitment VolumeCommitmentMapper(IDataReader reader)
        {
            VolumeCommitment volumeCommitment = new VolumeCommitment
            {
                VolumeCommitmentId = (int)reader["ID"],
                Settings = Vanrise.Common.Serializer.Deserialize<VolumeCommitmentSettings>(reader["Settings"] as string)
            };
            return volumeCommitment;
        }

        #endregion
    }
}
