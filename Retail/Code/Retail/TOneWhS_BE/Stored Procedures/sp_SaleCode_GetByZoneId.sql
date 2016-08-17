-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleCode_GetByZoneId]
	-- Add the parameters for the stored procedure here
	@ZoneID bigint,
	@When DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
/****** Script for SelectTopNRows command from SSMS  ******/
	SELECT  [ID],[Code],[ZoneID],[BED],[EED]
	FROM	[TOneWhS_BE].[SaleCode] sc with(nolock)
	WHERE	[ZoneID]=@ZoneID
			and ((sc.BED <= @when ) and (sc.EED is null or sc.EED > @when))
        
END