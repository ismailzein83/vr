CREATE PROCEDURE [LCR].[sp_Code_GetActiveSuppliersInfo]
	@CodeEffectiveAfter datetime,
	@CodeEffectiveOn datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	WITH UpdatedSupIDs as (SELECT distinct Z.SupplierID
							FROM Code C WITH(NOLOCK)
							JOIN Zone Z WITH(NOLOCK) ON  C.ZoneID = Z.ZoneID 
							WHERE  ((C.BeginEffectiveDate > @CodeEffectiveAfter OR @CodeEffectiveAfter IS NULL)
										AND C.BeginEffectiveDate <= @CodeEffectiveOn)
									 OR
								   ((C.EndEffectiveDate > @CodeEffectiveAfter OR @CodeEffectiveAfter IS NULL)
										AND C.EndEffectiveDate <= @CodeEffectiveOn))
   SELECT CarrierAccountID, 
			CASE 
			WHEN upCA.SupplierID IS NOT NULL THEN 1 
			ELSE 0 END AS HasCodeUpdated
    FROM CarrierAccount ca  WITH(NOLOCK)
    LEFT JOIN UpdatedSupIDs upCA ON ca.CarrierAccountID = upCA.SupplierID 
    WHERE 
            ca.IsDeleted = 'N' 
        AND ca.ActivationStatus IN (2, 1) --(byte)ActivationStatus.Active, (byte)ActivationStatus.Testing
        AND ca.RoutingStatus IN (3, 1) --(byte)RoutingStatus.Enabled, (byte)RoutingStatus.BlockedInbound
	
	
	
	--SELECT Z.SupplierID, MAX(CASE WHEN  (C.BeginEffectiveDate > @CodeEffectiveAfter AND C.BeginEffectiveDate <= @CodeEffectiveOn)
	--										OR
	--									  (C.EndEffectiveDate > @CodeEffectiveAfter AND C.EndEffectiveDate <= @CodeEffectiveOn)
	--							  THEN 1 ELSE 0 END) HasCodeUpdated
	--FROM Code C WITH(NOLOCK)
	--JOIN Zone Z WITH(NOLOCK) ON  C.ZoneID = Z.ZoneID 
	--GROUP BY Z.SupplierID
	
	
	
	--SELECT CarrierAccountID, 
	--		CASE 
	--		WHEN EXISTS (SELECT TOP 1 1 
	--						FROM Code C WITH(NOLOCK)
	--						JOIN Zone Z WITH(NOLOCK) ON  C.ZoneID = Z.ZoneID 
	--						WHERE Z.SupplierID = ca.CarrierAccountID
	--							  AND
	--							  (
	--								  (C.BeginEffectiveDate > @CodeEffectiveAfter AND C.BeginEffectiveDate <= @CodeEffectiveOn)
	--									OR
	--								  (C.EndEffectiveDate > @CodeEffectiveAfter AND C.EndEffectiveDate <= @CodeEffectiveOn)
	--							  )
	--					) THEN 1 
	--		ELSE 0 END AS HasCodeUpdated
 --   FROM CarrierAccount ca  WITH(NOLOCK)
 --   WHERE 
 --           ca.IsDeleted = 'N' 
 --       AND ca.ActivationStatus IN (2, 1) --(byte)ActivationStatus.Active, (byte)ActivationStatus.Testing
 --       AND ca.RoutingStatus IN (3, 1) --(byte)RoutingStatus.Enabled, (byte)RoutingStatus.BlockedInbound
	
	
END