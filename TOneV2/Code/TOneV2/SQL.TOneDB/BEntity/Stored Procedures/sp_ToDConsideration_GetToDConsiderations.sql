
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_ToDConsideration_GetToDConsiderations]
	@zoneId int,
	@customerId varchar(10),
	@when datetime
AS
BEGIN
SELECT  tod.ToDConsiderationID as ID ,
		tod.[ZoneID] as ZoneID,
		z.name,
		--z.supplierid,
		--z.codegroup,
		--z.BeginEffectiveDate,
		--z.EndEffectiveDate,
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
		--tod.[IsEffective],
		--[IsActive],
		--tod.[UserID],
		tod.[timestamp]
		FROM [ToDConsideration] tod with (nolock) inner join Zone z  with (nolock) on tod.ZoneID=z.ZoneID
		Where  (tod.EndEffectiveDate is null or (tod.EndEffectiveDate > @when and tod.BeginEffectiveDate<tod.EndEffectiveDate))
		and (z.EndEffectiveDate is null or (z.EndEffectiveDate > @when and z.BeginEffectiveDate<z.EndEffectiveDate))
		and (tod.CustomerID = @customerId) and (z.ZoneID=@zoneId)
		order by z.Name
		
END