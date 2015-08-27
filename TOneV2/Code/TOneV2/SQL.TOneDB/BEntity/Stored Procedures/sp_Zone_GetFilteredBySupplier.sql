

CREATE PROCEDURE [BEntity].[sp_Zone_GetFilteredBySupplier]
	@SupplierId varchar(5),
	@NameFilter nvarchar(255),
	@WhenDate Datetime
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
     and z.BeginEffectiveDate <= @WhenDate and (z.EndEffectiveDate is NULL or z.EndEffectiveDate > @WhenDate)
     ORDER BY Z.Name
END