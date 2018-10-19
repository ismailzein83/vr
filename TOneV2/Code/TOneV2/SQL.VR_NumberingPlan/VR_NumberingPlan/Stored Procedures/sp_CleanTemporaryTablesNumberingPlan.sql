-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_NumberingPlan].sp_CleanTemporaryTablesNumberingPlan
@ProcessInstanceId bigint
AS
BEGIN
	delete  from [VR_NumberingPlan].CP_SaleCode_Changed
	where ProcessInstanceID = @ProcessInstanceId

	delete  from  [VR_NumberingPlan].CP_SaleCode_New
	where ProcessInstanceID = @ProcessInstanceId
	
	delete  from  [VR_NumberingPlan].CP_SaleZone_Changed
	where ProcessInstanceID = @ProcessInstanceId

	delete  from  [VR_NumberingPlan].CP_SaleZone_New
	where ProcessInstanceID = @ProcessInstanceId
	
	delete  from  [VR_NumberingPlan].SaleCode_Preview
	where ProcessInstanceID = @ProcessInstanceId
	
	delete  from  [VR_NumberingPlan].SaleZone_Preview
	where ProcessInstanceID = @ProcessInstanceId

END