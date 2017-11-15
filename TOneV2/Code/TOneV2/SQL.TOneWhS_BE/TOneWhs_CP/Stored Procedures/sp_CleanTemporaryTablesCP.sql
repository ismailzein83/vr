-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [TOneWhs_CP].[sp_CleanTemporaryTablesCP]
@ProcessInstanceId bigint
AS
BEGIN
	delete  from  [TOneWhS_BE].CP_CustomerCountry_Changed
	where ProcessInstanceID = @ProcessInstanceId

	delete  from  [TOneWhS_BE].CP_SaleCode_Changed
	where ProcessInstanceID = @ProcessInstanceId

	delete  from  [TOneWhS_BE].CP_SaleCode_New
	where ProcessInstanceID = @ProcessInstanceId

	delete  from  [TOneWhS_BE].CP_SaleRate_Changed
	where ProcessInstanceID = @ProcessInstanceId

	delete  from  [TOneWhS_BE].CP_SaleRate_New
	where ProcessInstanceID = @ProcessInstanceId

	delete  from  [TOneWhS_BE].CP_SaleZone_Changed
	where ProcessInstanceID = @ProcessInstanceId

	delete  from  [TOneWhS_BE].CP_SaleZone_New
	where ProcessInstanceID = @ProcessInstanceId

	delete  from  [TOneWhS_BE].CP_SaleZoneRoutingProduct_New
	where ProcessInstanceID = @ProcessInstanceId

	delete  from  [TOneWhS_BE].CP_SaleZoneRoutingProducts_Changed
	where ProcessInstanceID = @ProcessInstanceId

	delete  from  [TOneWhS_BE].CP_SaleZoneServices_Changed
	where ProcessInstanceID = @ProcessInstanceId

	delete  from  [TOneWhS_CP].SaleCode_Preview
	where ProcessInstanceID = @ProcessInstanceId

	delete  from  [TOneWhS_CP].SaleRate_Preview
	where ProcessInstanceID = @ProcessInstanceId

	delete  from  [TOneWhS_CP].SaleZone_Preview
	where ProcessInstanceID = @ProcessInstanceId

	delete  from  [TOneWhS_CP].SaleZoneRoutingProduct_Preview
	where ProcessInstanceID = @ProcessInstanceId

	delete  from  [TOneWhS_BE].SalePriceList_New
	where ProcessInstanceID = @ProcessInstanceId

	delete  from  [TOneWhS_BE].SalePricelistCodeChange_New
	where BatchID = @ProcessInstanceId

	delete  from  [TOneWhS_BE].SalePricelistCustomerChange_New
	where BatchID = @ProcessInstanceId

	delete  from  [TOneWhS_BE].SalePricelistRateChange_New
	where ProcessInstanceID = @ProcessInstanceId

	delete  from  [TOneWhS_BE].SalePricelistRPChange_New
	where ProcessInstanceID = @ProcessInstanceId
END