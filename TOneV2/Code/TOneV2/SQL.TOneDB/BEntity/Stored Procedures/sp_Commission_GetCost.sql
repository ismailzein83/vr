-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Commission_GetCost]
	@When datetime
AS
BEGIN
	SELECT	[CommissionID]		as ID ,
		c.[SupplierID]		as supplierId,
		[CustomerID]		as customerId,
		c.[ZoneID]			as zoneId,
		z.name				as zoneName,
		z.supplierid		as zoneSupplierId,
		z.codegroup			as codegroupId,
		z.BeginEffectiveDate as zoneBED ,
		z.EndEffectiveDate	as zoneEED,
		[FromRate]			as fromRate,
		[ToRate]			as toRate,
		[Percentage]		as percentage,
		[Amount]			as amount,
		c.[BeginEffectiveDate]	as BED,
		c.[EndEffectiveDate]	as EED,
		[IsExtraCharge]			as isExtraCharge,
		c.[IsEffective]			as isEffective,
		c.[UserID]				as userId
	FROM [Commission] c with (nolock) inner join Zone z with (nolock) on c.ZoneID=z.ZoneID 
	where(c.BeginEffectiveDate<=@When and (c.EndEffectiveDate is NULL or  c.EndEffectiveDate>@When))
		and (z.BeginEffectiveDate<=@When and (z.EndEffectiveDate is NULL or  z.EndEffectiveDate>@When))
		and (c.SupplierID<>'SYS')
	order by z.Name
END