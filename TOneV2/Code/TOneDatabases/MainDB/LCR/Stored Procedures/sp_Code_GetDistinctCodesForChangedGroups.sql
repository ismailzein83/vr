CREATE PROCEDURE [LCR].[sp_Code_GetDistinctCodesForChangedGroups]
	@IsFuture bit = 0,
	@EffectiveOn datetime,
	@GetChangeGroupsOnly bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @Today datetime
	SELECT @Today = DATEADD(day,DATEDIFF(day,0,GETDATE()),0);
	
	select Code into #OrderedCodeGroup FROM CodeGroup ORDER BY Code desc;
	
	WITH CodeWithGroup AS
	(SELECT c.*, (SELECT TOP 1 Code From #OrderedCodeGroup cg WHERE c.Code like cg.Code + '%') CodeGroup
	  FROM [Code] c with (nolock)
	  where IsEffective = 'Y'),
	
	changedGroups AS (SELECT distinct C.CodeGroup
							  FROM CodeWithGroup C WITH (NOLOCK)
							  WHERE @GetChangeGroupsOnly = 0 OR c.BeginEffectiveDate = @EffectiveOn OR c.EndEffectiveDate = @EffectiveOn)
	
	SELECT distinct c.Code
    FROM CodeWithGroup C 
    JOIN Zone Z WITH(NOLOCK) ON C.ZoneID = Z.ZoneID 
    JOIN changedGroups grp WITH (NOLOCK) ON C.CodeGroup = grp.CodeGroup
    WHERE 
        ( 
			(@IsFuture = 0 
				AND C.[BeginEffectiveDate] <= @EffectiveOn  AND (C.EndEffectiveDate IS NULL OR C.EndEffectiveDate > @EffectiveOn)
				AND Z.[BeginEffectiveDate] <= @EffectiveOn  AND (Z.EndEffectiveDate IS NULL OR Z.EndEffectiveDate > @EffectiveOn)
			)
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
END