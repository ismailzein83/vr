-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierCode_GetParentsBySupplier]
@supplierId int,
@code varchar(20)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT  sc.[ID],sc.[Code],sc.[ZoneID],sc.[CodeGroupID],sc.[BED],sc.[EED],sc.[SourceID]
    from	[TOneWhS_BE].SupplierCode sc WITH(NOLOCK)
	Inner Join  [TOneWhS_BE].SupplierZone sz on sz.ID = sc.ZoneID
	WHERE  @code  like Code  + '%'
	 AND sz.SupplierID = @supplierId
	  AND   sc.BED <= getdate()
	  AND   (sc.EED IS NULL OR sc.EED > getDate())
	ORDER BY Code
END