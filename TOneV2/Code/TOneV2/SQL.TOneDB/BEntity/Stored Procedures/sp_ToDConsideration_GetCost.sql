-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [BEntity].[sp_ToDConsideration_GetCost]
	@when datetime
AS
BEGIN
	SELECT  tod.ToDConsiderationID as ID ,
			tod.[ZoneID] as ZoneID,
			z.name,
			tod.[SupplierID] as SupplierID,
			tod.[CustomerID] as CustomerID,
			[BeginTime] as BeginTime,
			[EndTime] as EndTime,
			[WeekDay] as TODWeekDay,
			[HolidayDate] as HolidayDate ,
			[HolidayName] as HolidayName,
			[RateType] as RateType,
			tod.[BeginEffectiveDate] as BeginEffectiveDate ,
			tod.[EndEffectiveDate] as EndEffectiveDate,
			tod.[timestamp]
	FROM	[ToDConsideration] tod with (nolock) inner join Zone z  with (nolock) on tod.ZoneID=z.ZoneID
	Where	(tod.BeginEffectiveDate<=@when AND  (tod.EndEffectiveDate IS NULL or tod.EndEffectiveDate>@when))
			AND (z.BeginEffectiveDate<=@when AND  (z.EndEffectiveDate IS NULL or z.EndEffectiveDate>@when))
			and (tod.SupplierID<>'SYS')
	ORDER BY z.Name
END