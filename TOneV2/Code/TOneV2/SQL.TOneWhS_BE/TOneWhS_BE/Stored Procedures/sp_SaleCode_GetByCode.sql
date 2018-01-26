-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleCode_GetByCode]
@code varchar(20)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT  sc.[ID],sc.[Code],sc.[ZoneID],sc.[CodeGroupID],sc.[BED],sc.[EED],sc.[SourceID]
    from	[TOneWhS_BE].SaleCode sc WITH(NOLOCK) 
	WHERE   Code like  @code + '%'
	  AND   BED <= getdate()
	  AND   (EED IS NULL OR EED > getDate())
	ORDER BY Code
END