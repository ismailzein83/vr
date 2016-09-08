-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleCode_GetByCodePrefix]
	-- Add the parameters for the stored procedure here
	@CodePrefix varchar(20),
	@EffectiveOn DateTime,
	@IsFuture BIT,
	@GetChildCodes bit,
	@GetParentCodes bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
/****** Script for SelectTopNRows command from SSMS  ******/
	SELECT  sc.[ID],
			sc.[Code],
			sc.[ZoneID],
			sc.[BED],
			sc.[EED]
	FROM	[TOneWhS_BE].[SaleCode] sc WITH(NOLOCK) 
	JOIN	[TOneWhS_BE].[SaleZone] sz  WITH(NOLOCK) ON sc.ZoneID=sz.ID
	WHERE  ((sc.[Code] like @CodePrefix + '%' And @GetChildCodes = 1) OR (@CodePrefix like sc.Code + '%'  And @GetParentCodes = 1))
	   AND ((@IsFuture = 0 AND sc.BED <= @EffectiveOn AND  (sc.EED > @EffectiveOn OR sc.EED IS NULL))
	   OR (@IsFuture = 1 AND (sc.BED > GETDATE() OR sc.EED IS NULL)))
END