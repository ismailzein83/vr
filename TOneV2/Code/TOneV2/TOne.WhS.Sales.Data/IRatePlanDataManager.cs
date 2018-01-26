using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Data
{
    public interface IRatePlanDataManager : IDataManager
    {
        bool InsertPriceList(SalePriceList priceList, out int priceListId);
        bool InsertSalePriceList(SalePriceListOwnerType ownerType, int ownerId, int currencyId, DateTime effectiveOn, out int salePriceListId);
        Changes GetChanges(SalePriceListOwnerType ownerType, int ownerId, RatePlanStatus status);
        bool InsertOrUpdateChanges(SalePriceListOwnerType ownerType, int ownerId, Changes changes, RatePlanStatus status);
        bool UpdateRatePlanStatus(SalePriceListOwnerType ownerType, int ownerId, RatePlanStatus existingStatus, RatePlanStatus newStatus);
        bool CancelRatePlanChanges(SalePriceListOwnerType ownerType, int ownerId);
        bool SyncImportedDataWithDB(long processInstanceId, int? salePriceListId, SalePriceListOwnerType ownerType, int ownerId, int currencyId, DateTime effectiveOn, long stateBackupId);
        bool DeleteRatePlanDraft(SalePriceListOwnerType ownerType, int ownerId);
        DraftTaskData GetDraftTaskData(SalePriceListOwnerType ownerType, int ownerId, RatePlanStatus status);
        bool InsertOrUpdateDraftTaskData(SalePriceListOwnerType ownerType, int ownerId, DraftTaskData draftTaskData, RatePlanStatus status);
        bool CleanTemporaryTables(long processInstanceId);
        void SetStateBackupIdForOwnerPricelists(long processInstanceId, SalePriceListOwnerType ownerType, int ownerId, long stateBackupId);
    }
}
