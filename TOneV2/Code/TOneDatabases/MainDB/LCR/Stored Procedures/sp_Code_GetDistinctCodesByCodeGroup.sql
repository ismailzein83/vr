CREATE PROCEDURE [LCR].[sp_Code_GetDistinctCodesByCodeGroup]
	@IsFuture bit = 0,
	@CodeGroup varchar(20)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @Today datetime
	SELECT @Today = DATEADD(day,DATEDIFF(day,0,GETDATE()),0)
	
	SELECT distinct c.Code
    FROM Code C WITH(NOLOCK), Zone Z WITH(NOLOCK),CarrierAccount CA WITH(NOLOCK)
    WHERE Z.CodeGroup = @CodeGroup
		AND C.ZoneID = Z.ZoneID 
        AND Z.SupplierID = CA.CarrierAccountID AND CA.ActivationStatus = 2 --{1}
        AND
        ( 
			(@IsFuture = 0 AND C.IsEffective='Y' AND Z.IsEffective='Y' )
		OR
			(@IsFuture = 1 AND (C.EndEffectiveDate IS NULL OR C.[BeginEffectiveDate] > @Today) AND (Z.EndEffectiveDate IS NULL OR Z.[BeginEffectiveDate] > @Today))
		) 
END