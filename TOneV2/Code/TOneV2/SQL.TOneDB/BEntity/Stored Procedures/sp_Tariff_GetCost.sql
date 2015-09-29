-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Tariff_GetCost]
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
	Where  (t.BeginEffectiveDate<=@when and (t.EndEffectiveDate is null or t.EndEffectiveDate > @when))
			and (z.BeginEffectiveDate<=@when and  (z.EndEffectiveDate is null or z.EndEffectiveDate > @when))
			and (t.SupplierID<>'SYS')
			order by z.Name
END