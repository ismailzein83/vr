

CREATE PROCEDURE [BEntity].[sp_Zone_GetFilteredBySupplierId]
	@SupplierId varchar(5),
	@NameFilter nvarchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
    SELECT Z.ZoneID
		  ,Z.Name
    FROM Zone Z WITH(NOLOCK)    
    WHERE   
     Z.SupplierID = @SupplierId and Z.Name like('%'+ @NameFilter+'%')
END