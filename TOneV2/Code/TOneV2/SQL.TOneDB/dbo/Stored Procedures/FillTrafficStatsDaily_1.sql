-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FillTrafficStatsDaily]
AS
BEGIN
	SET NOCOUNT ON;

INSERT INTO [TONE-ProtelMarch2012].[dbo].[TrafficStatsDaily]
           ([Calldate]
           ,[SwitchId]
           ,[CustomerID]
           ,[OurZoneID]
           ,[SupplierID]
           ,[SupplierZoneID]
           ,[Attempts]
           ,[DeliveredAttempts]
           ,[SuccessfulAttempts]
           ,[DurationsInSeconds]
           ,[PDDInSeconds]
           ,[UtilizationInSeconds]
           ,[NumberOfCalls]
           ,[DeliveredNumberOfCalls])
     SELECT convert(varchar(10),[FirstCDRAttempt],120)
      ,[SwitchId]
      ,[CustomerID]
      ,[OurZoneID]
      ,[SupplierID]
      ,[SupplierZoneID]
      ,Sum([Attempts])
      ,Sum([DeliveredAttempts])
      ,Sum([SuccessfulAttempts])
      ,Sum([DurationsInSeconds])
      ,AVG([PDDInSeconds])
      ,Sum([UtilizationInSeconds])
      ,Sum([NumberOfCalls])
      ,Sum([DeliveredNumberOfCalls])
  FROM [TrafficStats]
Where FirstCDRAttempt>=DATEADD(dd,-1, convert(varchar(10),GETDATE(),121))
Group By convert(varchar(10),[FirstCDRAttempt],120)
		,[SwitchId]
		,[CustomerID]
		,[OurZoneID]
		,[SupplierID]
		,[SupplierZoneID]

END