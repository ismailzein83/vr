
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Commission_GetCommissions]
	@zoneId int,
	@customerId varchar(10),
	@EndEffectiveDate datetime
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
	where c.EndEffectiveDate is null or c.EndEffectiveDate> @EndEffectiveDate
		and (z.EndEffectiveDate is null or (z.EndEffectiveDate > @EndEffectiveDate and z.BeginEffectiveDate<z.EndEffectiveDate))
		and (c.CustomerID = @customerId)
		and (z.ZoneID= @zoneId)
	order by z.Name

END