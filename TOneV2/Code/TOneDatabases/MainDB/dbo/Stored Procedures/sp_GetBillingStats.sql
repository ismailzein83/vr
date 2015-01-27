CREATE proc [dbo].[sp_GetBillingStats]
(
@P1 datetime,
@P2 datetime
)
as
select * from Billing_Stats where calldate between @P1 and @P2