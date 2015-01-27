CREATE PROCEDURE [LCR].[sp_Code_GetBySupplier]
	@SupplierID varchar(5),
	@EffectiveOn datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SELECT 
       c.[ID]
      ,c.[Code]
      ,c.[ZoneID]
      ,c.[BeginEffectiveDate]
      ,c.[EndEffectiveDate]
      ,c.[IsEffective]
      ,c.[UserID]
      ,c.[timestamp]
      ,z.CodeGroup 
      ,z.SupplierID
    FROM Code C WITH(NOLOCK), Zone Z WITH(NOLOCK)--,CarrierAccount CA
    WHERE C.ZoneID = Z.ZoneID 
        AND Z.SupplierID = @SupplierID-- AND CA.ActivationStatus = 2 --{1}
        AND (C.EndEffectiveDate IS NULL OR (C.EndEffectiveDate > @EffectiveOn And C.BeginEffectiveDate<>C.EndEffectiveDate)) 
        AND (Z.EndEffectiveDate IS NULL OR (Z.EndEffectiveDate > @EffectiveOn And Z.BeginEffectiveDate<>Z.EndEffectiveDate)) 
    --ORDER BY Z.SupplierID, C.Code, C.BeginEffectiveDate DESC
END