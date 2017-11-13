-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CleanTemporaryTableSPL]
@ProcessInstanceId bigint
AS
BEGIN
delete  from  [TOneWhS_BE].SPL_SupplierCode_Changed
where ProcessInstanceID = @ProcessInstanceId

delete  from  [TOneWhS_BE].SPL_SupplierCode_New
where ProcessInstanceID = @ProcessInstanceId

delete  from  [TOneWhS_BE].SPL_SupplierRate_Changed
where ProcessInstanceID = @ProcessInstanceId

delete  from  [TOneWhS_BE].SPL_SupplierRate_New
where ProcessInstanceID = @ProcessInstanceId

delete  from  [TOneWhS_BE].SPL_SupplierZone_Changed
where ProcessInstanceID = @ProcessInstanceId

delete  from  [TOneWhS_BE].SPL_SupplierZone_New
where ProcessInstanceID = @ProcessInstanceId

delete  from  [TOneWhS_BE].SPL_SupplierZoneService_Changed
where ProcessInstanceID = @ProcessInstanceId

delete  from  [TOneWhS_BE].SPL_SupplierZoneService_New
where ProcessInstanceID = @ProcessInstanceId
END