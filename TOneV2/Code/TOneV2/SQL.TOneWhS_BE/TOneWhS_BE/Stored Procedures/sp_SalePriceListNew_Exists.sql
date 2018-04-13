
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePriceListNew_Exists]

@ProcessInstanceId as bigint
AS
BEGIN
	
	Declare @PriceListExists bit
	set @PriceListExists = 0;

	IF Exists(select top 1 [ProcessInstanceID] from [TOneWhS_BE].SalePriceList_New where ProcessInstanceID = @ProcessInstanceId)
	Begin
	 SET @PriceListExists = 1;
	END
	
	Select @PriceListExists as PriceListExists
END