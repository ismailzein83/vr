-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleCode_Update]
	-- Add the parameters for the stored procedure here
	@SaleCodes [TOneWhS_BE].[SaleData] READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE [TOneWhS_BE].[SaleCode]
	SET 
		[TOneWhS_BE].[SaleCode].EED=sc.EED
	FROM [TOneWhS_BE].[SaleCode]  inner join @SaleCodes as sc ON  [TOneWhS_BE].[SaleCode].ID = sc.ID
END