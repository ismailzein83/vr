CREATE PROCEDURE [LCR].[sp_Code_GetForActiveSuppliersByCodeGroup]
	@IsFuture bit = 0,
	@CodeGroup varchar(20)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @Today datetime
	SELECT @Today = DATEADD(day,DATEDIFF(day,0,GETDATE()),0)
	
    SELECT c.[ID]
		  ,c.[Code]
		  ,c.[ZoneID]
		  ,c.[BeginEffectiveDate]
		  ,c.[EndEffectiveDate]
		  ,c.[IsEffective]
		  ,c.[UserID]
		  ,c.[timestamp]
		  ,z.CodeGroup 
		  ,z.SupplierID
    FROM Code C WITH(NOLOCK)
    JOIN Zone Z WITH(NOLOCK) ON  C.ZoneID = Z.ZoneID 
    WHERE 
		Z.CodeGroup = @CodeGroup
		AND
		( 
			(@IsFuture = 0 AND C.IsEffective='Y' AND Z.IsEffective='Y' )
		OR
			(@IsFuture = 1 AND (C.EndEffectiveDate IS NULL OR C.[BeginEffectiveDate] > @Today) AND (Z.EndEffectiveDate IS NULL OR Z.[BeginEffectiveDate] > @Today))
		) 
        AND Z.SupplierID IN 
            (
                SELECT CarrierAccountID 
                    FROM CarrierAccount ca  WITH(NOLOCK)
                    WHERE 
                            ca.IsDeleted = 'N' 
                        AND ca.ActivationStatus IN (2, 1) --(byte)ActivationStatus.Active, (byte)ActivationStatus.Testing
                        AND ca.RoutingStatus IN (3, 1) --(byte)RoutingStatus.Enabled, (byte)RoutingStatus.BlockedInbound
            )
	ORDER BY Z.SupplierID, C.Code desc
END