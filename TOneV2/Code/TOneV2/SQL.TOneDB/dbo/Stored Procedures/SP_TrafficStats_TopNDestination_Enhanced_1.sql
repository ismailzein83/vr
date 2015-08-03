





CREATE PROCEDURE [dbo].[SP_TrafficStats_TopNDestination_Enhanced]
 @TopRecord    INT,
 @FromDate     DateTime,
 @ToDate       DateTime,
 @sortOrder   VARCHAR(25) = 'DESC',
 @CustomerID varchar (25)= NULL,
 @SupplierID varchar (25) = NULL,
 @SwitchID	 varchar(50) = NULL,
 @ShowSupplier Char(1) = 'N'	
AS
BEGIN
	SET ROWCOUNT @TopRecord

--	Set @CustomerID = ISNULL(@CustomerID, '')
--	Set @SupplierID = ISNULL(@SupplierID, '')
    SET @SwitchID =isnull(@SwitchID,'')
	SET NOCOUNT ON

	Declare @FromDateStr varchar(50)
	Declare @ToDateStr  varchar(50)

	SELECT @FromDateStr = CONVERT(varchar(50), @FromDate, 121)
	SELECT @ToDateStr = CONVERT(varchar(50), @ToDate, 121)

	DECLARE @Sql varchar(8000)
if @CustomerID is null and @SupplierID is null
	SELECT @Sql =
		'SELECT  S.OurZoneID,
					Z.[Name] AS [Name],
					CASE WHEN '''+@ShowSupplier+'''=''Y'' THEN S.SupplierID  ELSE NULL END AS SupplierID,
					SUM(NumberOfCalls) As Attempts,
					SUM(DurationsInSeconds) / 60.0 as DurationsInMinutes,
					case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
					Sum(SuccessfulAttempts)as SuccessfulAttempt,
					case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
					case when Sum(NumberOfCalls) > 0 then Sum(deliveredAttempts) * 100.0 / SUM(NumberOfCalls) ELSE 0 end as DeliveredASR,
				    Avg(PDDinSeconds) as AveragePDD
					FROM TrafficStats S WITH(NOLOCK)
						join Zone Z WITH(nolock) on S.OurZoneID=Z.ZoneID
						LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON S.CustomerID = CA.CarrierAccountID
					WHERE 
									(
										FirstCDRAttempt BETWEEN '''+@FromDateStr+''' AND '''+@ToDateStr+'''
									)
							        and(('''+@SwitchID+ '''=''''  AND CustomerID IS NOT NULL AND CA.RepresentsASwitch=''' + 'N' + ''' )   OR S.SwitchID ='''+@SwitchID +''')
		 GROUP BY S.OurZoneID,
				  Z.[Name],
				  CASE WHEN '''+@ShowSupplier+'''=''Y'' THEN S.SupplierID  ELSE NULL END 
		 ORDER BY DurationsInMinutes '+ @sortOrder + ',
				  Attempts DESC'


if @CustomerID is not null and @SupplierID is null
	SELECT @Sql =
		'SELECT  S.OurZoneID,
					Z.[Name] AS [Name],
					CASE WHEN '''+@ShowSupplier+'''=''Y'' THEN S.SupplierID  ELSE NULL END AS SupplierID,
					SUM(NumberOfCalls) As Attempts,
					SUM(DurationsInSeconds) / 60.0 as DurationsInMinutes,
					case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
					Sum(SuccessfulAttempts)as SuccessfulAttempt,
					case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
					case when Sum(NumberOfCalls) > 0 then Sum(deliveredAttempts) * 100.0 / SUM(NumberOfCalls) ELSE 0 end as DeliveredASR,
				    Avg(PDDinSeconds) as AveragePDD
					FROM TrafficStats S WITH(NOLOCK)
						join Zone Z WITH(nolock) on S.OurZoneID=Z.ZoneID
						LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON S.CustomerID = CA.CarrierAccountID
					WHERE 
									(
										FirstCDRAttempt BETWEEN '''+@FromDateStr+''' AND '''+@ToDateStr+'''
									)
									and(S.CustomerID ='''+@CustomerID+''')
							        and(('''+@SwitchID+ '''=''''  AND CustomerID IS NOT NULL AND CA.RepresentsASwitch=''' + 'N' + ''' )   OR S.SwitchID ='''+@SwitchID +''')
					GROUP BY S.OurZoneID,
							 Z.[Name],
							 CASE WHEN '''+@ShowSupplier+''' =''Y'' THEN S.SupplierID  ELSE NULL END 
					ORDER BY DurationsInMinutes '+ @sortOrder + ',
							 Attempts DESC'

if @CustomerID is null and @SupplierID is not null
	SELECT @Sql =
		'SELECT  S.OurZoneID,
					Z.[Name] AS [Name],
					CASE WHEN '''+@ShowSupplier+'''=''Y'' THEN S.SupplierID  ELSE NULL END AS SupplierID,
					SUM(Attempts) As Attempts,
					SUM(DurationsInSeconds) / 60.0 as DurationsInMinutes,
					case when Sum(Attempts) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) ELSE 0 end as ASR,
					Sum(SuccessfulAttempts)as SuccessfulAttempt,
					case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
					case when Sum(Attempts) > 0 then Sum(deliveredAttempts) * 100.0 / SUM(Attempts) ELSE 0 end as DeliveredASR,
				    Avg(PDDinSeconds) as AveragePDD
					FROM TrafficStats S WITH(NOLOCK)
						join Zone Z WITH(nolock) on S.OurZoneID=Z.ZoneID
						LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON S.CustomerID = CA.CarrierAccountID
					WHERE 
									(
										FirstCDRAttempt BETWEEN '''+@FromDateStr+''' AND '''+@ToDateStr+'''
									)
									and(S.SupplierID ='''+@SupplierID+''')
							         and(('''+@SwitchID+ '''=''''  AND CustomerID IS NOT NULL AND CA.RepresentsASwitch=''' + 'N' + ''' )   OR S.SwitchID ='''+@SwitchID +''')
					GROUP BY S.OurZoneID,
					         Z.[Name],
					         CASE WHEN '''+@ShowSupplier+'''=''Y'' THEN S.SupplierID  ELSE NULL END 
					ORDER BY DurationsInMinutes '+ @sortOrder + ',
					         Attempts DESC'

if @CustomerID is not null and @SupplierID is not null
	SELECT @Sql =
		'SELECT  S.OurZoneID,
					Z.[Name] AS [Name],
					CASE WHEN '''+@ShowSupplier+'''=''Y'' THEN S.SupplierID  ELSE NULL END AS SupplierID,
					SUM(Attempts) As Attempts,
					SUM(DurationsInSeconds) / 60.0 as DurationsInMinutes,
					case when Sum(Attempts) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) ELSE 0 end as ASR,
					Sum(SuccessfulAttempts)as SuccessfulAttempt,
					case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
					case when Sum(Attempts) > 0 then Sum(deliveredAttempts) * 100.0 / SUM(Attempts) ELSE 0 end as DeliveredASR,
				    Avg(PDDinSeconds) as AveragePDD
					FROM TrafficStats S WITH(NOLOCK)
						join Zone Z WITH(nolock) on S.OurZoneID=Z.ZoneID
						LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON S.CustomerID = CA.CarrierAccountID
					WHERE 
									(
										FirstCDRAttempt BETWEEN '''+@FromDateStr+''' AND '''+@ToDateStr+'''
									)
							        and(('''+@SwitchID+ '''=''''  AND CustomerID IS NOT NULL AND CA.RepresentsASwitch=''' + 'N' + ''' )   OR S.SwitchID ='''+@SwitchID +''')
					GROUP BY S.OurZoneID,
					         Z.[Name],
					         CASE WHEN '''+@ShowSupplier+'''=''Y'' THEN S.SupplierID  ELSE NULL END 
					ORDER BY DurationsInMinutes '+ @sortOrder + ',
					         Attempts DESC'

	EXECUTE(@Sql)
--    PRINT @Sql






	
END