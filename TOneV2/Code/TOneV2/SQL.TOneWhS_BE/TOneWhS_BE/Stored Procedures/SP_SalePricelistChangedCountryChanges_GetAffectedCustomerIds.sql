-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [TOneWhS_BE].[SP_SalePricelistChangedCountryChanges_GetAffectedCustomerIds]
	@ProcessInstanceId bigint
AS
BEGIN
	Select	Distinct CustomerID
	from	[TOneWhS_Sales].RP_CustomerCountry_ChangedPreview with(Nolock)
	where	ProcessInstanceID = @ProcessInstanceId
END