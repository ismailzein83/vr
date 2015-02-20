
CREATE PROCEDURE [BEntity].[sp_Zone_GetZoneInfoForActiveSuppliers]
	@ActiveSuppliersInfo BEntity.CarrierAccountInfoType READONLY,
	@EffectiveTime datetime,
	@IsFuture bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
    SELECT Z.ZoneID
		  ,Z.Name
    FROM Zone Z WITH(NOLOCK)
    JOIN @ActiveSuppliersInfo sup ON Z.SupplierID = sup.CarrierAccountID
    WHERE   (
				(@IsFuture = 0 AND Z.BeginEffectiveDate <= @EffectiveTime AND (Z.EndEffectiveDate IS NULL OR Z.EndEffectiveDate > @EffectiveTime))
				 OR
				(@IsFuture = 1 AND (Z.BeginEffectiveDate > GETDATE() OR Z.EndEffectiveDate IS NULL))
			)
END