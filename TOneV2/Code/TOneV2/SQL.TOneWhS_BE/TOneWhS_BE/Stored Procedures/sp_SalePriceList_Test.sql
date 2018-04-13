
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePriceList_Test]

@ProcessInstanceId as bigint
AS
BEGIN
	IF Exists( select top 1 [ProcessInstanceID] from [TOneWhS_BE].SalePriceList_New where ProcessInstanceID = @ProcessInstanceId)
	 return 1;
	 else
	 return 0;
END