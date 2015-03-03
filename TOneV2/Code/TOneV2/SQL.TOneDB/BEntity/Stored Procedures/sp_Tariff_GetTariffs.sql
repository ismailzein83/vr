
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Tariff_GetTariffs]
	@zoneId int,
	@customerId varchar(10),
	@when datetime
AS
BEGIN

SELECT  t.TariffID as TariffID,
		t.[ZoneID] as ZoneID,
		z.name ,
		z.supplierid,
		z.codegroup,
		z.BeginEffectiveDate,
		z.EndEffectiveDate,
		t.[CustomerID] as SupplierID,
		t.[SupplierID] as CustomerID,
		t.[CallFee] as CallFee,
		t.[FirstPeriodRate] as FirstPeriodRate,
		t.[FirstPeriod] as FirstPeriod,
		t.[RepeatFirstPeriod] as RepeatFirstPeriod,
		t.[FractionUnit] as FractionUnit,
		t.[BeginEffectiveDate],
		t.[EndEffectiveDate],
		t.[IsEffective],
		t.[UserID]
FROM  [Tariff] t  with (nolock) inner join Zone z  with (nolock) on t.ZoneID=z.ZoneID 
Where  (t.EndEffectiveDate is null or (t.EndEffectiveDate > @when and t.BeginEffectiveDate<t.EndEffectiveDate))
		and (z.EndEffectiveDate is null or (z.EndEffectiveDate > @when and z.BeginEffectiveDate<z.EndEffectiveDate))
		and (t.CustomerID = @customerId)
		and (z.ZoneID=@zoneId)
order by z.Name

END