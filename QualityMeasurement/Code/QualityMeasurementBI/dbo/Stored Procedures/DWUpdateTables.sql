-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DWUpdateTables]
	-- Add the parameters for the stored procedure here
      @fromdate datetime = null,
      @todate datetime = null
AS
BEGIN
/*SET DESTINATION DATABASES
Please make sure that database name match client databases

Replace [QualityMeasurement] with [ClearVoice main Database name]
Replace [QualityMeasurementConfiguration] with [ClearVoice configuration Database name]
Replace [QualityMeasurementTransactionDB] with [ClearVoice Transaction Database name]
*/

-- SET NOCOUNT ON added to prevent extra result sets from
-- interfering with SELECT statements.
SET NOCOUNT ON;

DECLARE @createdate DATETIME
DECLARE @duration INT
DECLARE @time VARCHAR(20)
DECLARE @from1 VARCHAR(12)

--1)Dim_Suppliers Table
SELECT @createdate=GETDATE()
RAISERROR ( 'Updating Dim_Suppliers Table...', 10, 0 ) WITH NOWAIT
--------------------------------------------------

MERGE Dim_Suppliers AS target
USING( 
	SELECT [ID],[Name],SUBSTRING(Settings, 94, CHARINDEX('"', SUBSTRING(Settings, 94, LEN(Settings))) - 1)
	FROM	[QualityMeasurement].[QM_BE].[Supplier]
	) AS Source([ID] ,[Name] ,[Prefix])
	ON ISNULL(target.[Pk_SupplierId],0)= ISNULL(source.[ID],0)	
	WHEN NOT MATCHED THEN
        INSERT ([Pk_SupplierId] ,[Name] ,[Prefix])
        VALUES (source.[ID] ,source.[Name] ,source.[Prefix])	
	WHEN MATCHED THEN 
		UPDATE SET target.Pk_SupplierId = source.ID , target.Name = source.Name , target.Prefix = source.Prefix;

SELECT @duration=DATEDIFF(SECOND,  @createdate,GETDATE())
SELECT @time = CONVERT(VARCHAR(5),@duration/3600) + 'h:' + CONVERT(VARCHAR(5),@duration%3600/60) + 'm:'+CONVERT(VARCHAR(5),(@duration%60)) + 's'
RAISERROR ( 'Dim_Suppliers Table created in: %s', 10, 0,@time ) WITH NOWAIT
-----------------------------------------------------------------------------------------------------------------------------------------------------------------
--2)Country Table
SELECT @createdate=GETDATE()
RAISERROR ( 'Updating Dim_Countries Table...', 10, 0 ) WITH NOWAIT
--------------------------------------------------
MERGE Dim_Countries AS targetCountry
	USING(
	SELECT [ID] ,[Name] FROM [QualityMeasurementConfiguration].[common].[Country]
	)
	AS SourceCountry([ID] ,[Name])
	ON ISNULL(targetCountry.[Pk_CountryId],0)= ISNULL(SourceCountry.[ID],0)
	WHEN NOT MATCHED THEN    
        INSERT ([Pk_CountryId] ,[Name])
        VALUES (SourceCountry.[ID] ,SourceCountry.[Name])
	WHEN MATCHED THEN 
		UPDATE SET targetCountry.Pk_CountryId = SourceCountry.ID , targetCountry.Name = SourceCountry.Name;

SELECT @duration=DATEDIFF(SECOND,  @createdate,GETDATE())
SELECT @time = CONVERT(VARCHAR(5),@duration/3600) + 'h:' + CONVERT(VARCHAR(5),@duration%3600/60) + 'm:'+CONVERT(VARCHAR(5),(@duration%60)) + 's'
RAISERROR ( 'Dim_Countries Table created in: %s', 10, 0,@time ) WITH NOWAIT
-----------------------------------------------------------------------------------------------------------------------------------------------------------------
--3)Zone Table
SELECT @createdate=GETDATE()
RAISERROR ( 'Updating Dim_Zones Table...', 10, 0 ) WITH NOWAIT
--------------------------------------------------
MERGE Dim_Zones AS targetZone
	USING(
	SELECT [ID] ,[Name] FROM [QualityMeasurement].[QM_BE].[Zone]
	)
	AS SourceZone([ID] ,[Name])
	ON ISNULL(targetZone.[Pk_ZoneId],0)= ISNULL(SourceZone.[ID],0)
	WHEN NOT MATCHED THEN    
        INSERT ([Pk_ZoneId] ,[Name])
        VALUES (SourceZone.[ID] ,SourceZone.[Name])
	WHEN MATCHED THEN 
		UPDATE SET targetZone.Pk_ZoneId = SourceZone.ID , targetZone.Name = SourceZone.Name;

SELECT @duration=DATEDIFF(SECOND,  @createdate,GETDATE())
SELECT @time = CONVERT(VARCHAR(5),@duration/3600) + 'h:' + CONVERT(VARCHAR(5),@duration%3600/60) + 'm:'+CONVERT(VARCHAR(5),(@duration%60)) + 's'
RAISERROR ( 'Dim_Zones Table created in: %s', 10, 0,@time ) WITH NOWAIT
-----------------------------------------------------------------------------------------------------------------------------------------------------------------
--4)Schedule Table
SELECT @createdate=GETDATE()
RAISERROR ( 'Updating Dim_Schedules Table...', 10, 0 ) WITH NOWAIT
--------------------------------------------------
MERGE Dim_Schedules AS targetSchedule
	USING(
	SELECT [ID] ,[Name] FROM [QualityMeasurementTransactionDB].[runtime].[ScheduleTask]
	)
	AS SourceSchedule([ID] ,[Name])
	ON ISNULL(targetSchedule.[Pk_ScheduleId],0)= ISNULL(SourceSchedule.[ID],0)
	
	WHEN NOT MATCHED THEN    
        INSERT ([Pk_ScheduleId] ,[Name])
        VALUES (SourceSchedule.[ID] ,SourceSchedule.[Name])
	WHEN MATCHED THEN 
		UPDATE SET targetSchedule.Pk_ScheduleId = SourceSchedule.ID , targetSchedule.Name = SourceSchedule.Name;

SELECT @duration=DATEDIFF(SECOND,  @createdate,GETDATE())
SELECT @time = CONVERT(VARCHAR(5),@duration/3600) + 'h:' + CONVERT(VARCHAR(5),@duration%3600/60) + 'm:'+CONVERT(VARCHAR(5),(@duration%60)) + 's'
RAISERROR ( 'Dim_Schedules Table created in: %s', 10, 0,@time ) WITH NOWAIT
-----------------------------------------------------------------------------------------------------------------------------------------------------------------
--5)User Table
SELECT @createdate=GETDATE()
RAISERROR ( 'Updating Dim_Users Table...', 10, 0 ) WITH NOWAIT
--------------------------------------------------
MERGE Dim_Users AS targetUser
	USING(
	SELECT [ID] ,[Name] FROM [QualityMeasurementConfiguration].[sec].[User]
	)
	AS SourceUser([ID] ,[Name])
	ON ISNULL(targetUser.[Pk_UserId],0)= ISNULL(SourceUser.[ID],0)
	
	WHEN NOT MATCHED THEN    
        INSERT ([Pk_UserId] ,[Name])
        VALUES (SourceUser.[ID] ,SourceUser.[Name])
	WHEN MATCHED THEN 
		UPDATE SET targetUser.Pk_UserId = SourceUser.ID , targetUser.Name = SourceUser.Name;

SELECT @duration=DATEDIFF(SECOND,  @createdate,GETDATE())
SELECT @time = CONVERT(VARCHAR(5),@duration/3600) + 'h:' + CONVERT(VARCHAR(5),@duration%3600/60) + 'm:'+CONVERT(VARCHAR(5),(@duration%60)) + 's'
RAISERROR ( 'Dim_Users Table created in: %s', 10, 0,@time ) WITH NOWAIT
-----------------------------------------------------------------------------------------------------------------------------------------------------------------
--6)Fact_TestCalls Table
IF @todate IS NULL
	BEGIN
		SELECT @createdate = GETDATE()
	END
ELSE
	BEGIN
		SELECT @createdate = @todate
	END

IF @fromdate IS NULL
	BEGIN
		SELECT @fromdate = DATEADD(DAY,-1,@createdate)
	END
RAISERROR ('updating Fact_TestCalls Table...', 10, 0) WITH NOWAIT
--------------------------------------------------

DECLARE @takeTime DATETIME
SELECT @takeTime = GETDATE()
SELECT @from1=CONVERT(VARCHAR(12),@fromdate)
RAISERROR ('Filling Fact_TestCalls for : %s'  , 10, 0,@from1) WITH NOWAIT

CREATE TABLE #FactTestCalls([ID] INT,[UserID] INT,[SupplierID] INT,[CountryID] INT  ,[ZoneID] INT ,[CallTestStatus] INT ,[CallTestResult] INT ,[ScheduleID] INT ,[CreationDate] DATETIME)

INSERT INTO #FactTestCalls 
SELECT	[ID],[UserID],[SupplierID],[CountryID],[ZoneID],[CallTestStatus],[CallTestResult],[ScheduleID],[CreationDate]
FROM	[QualityMeasurement].[QM_CLITester].[TestCall] WITH(NOLOCK)
WHERE	(CreationDate >= @fromdate AND CreationDate < DATEADD(DAY,1,@createdate))
   
IF NOT EXISTS(
	SELECT	DateInstance,DATEPART(yyyy,DateInstance),DATEPART(mm,DateInstance),DATEPART(ww,DateInstance),DATEPART(dd,DateInstance),DATEPART(hh,DateInstance),DATENAME(month,DateInstance),DATENAME(dw,DateInstance)
	FROM	Dim_Time
	WHERE	DateInstance IN (SELECT CreationDate FROM #FactTestCalls)
			)
		INSERT	INTO Dim_Time WITH(tablock)
		SELECT	DISTINCT DATEADD(hour, DATEDIFF(hour, 0, CreationDate), 0),DATEPART(yyyy,FactTCall.CreationDate),DATEPART(mm,FactTCall.CreationDate),DATEPART(ww,FactTCall.CreationDate),DATEPART(dd,FactTCall.CreationDate),DATEPART(hh,FactTCall.CreationDate),DATENAME(month,FactTCall.CreationDate),DATENAME(dw,FactTCall.CreationDate)
		FROM	#FactTestCalls FactTCall WITH(NOLOCK)

INSERT	INTO Fact_TestCalls WITH(tablock)
SELECT	FactTCall2.[ID],FactTCall2.[UserID],FactTCall2.[SupplierID],FactTCall2.[CountryID],FactTCall2.[ZoneID],FactTCall2.[CallTestStatus],FactTCall2.[CallTestResult],FactTCall2.[ScheduleID],FactTCall2.[CreationDate]
FROM	#FactTestCalls FactTCall2 WITH(NOLOCK) 
	   
DROP TABLE #FactTestCalls

SELECT @duration=DATEDIFF(SECOND, @takeTime,GETDATE())
SELECT @time = CONVERT(VARCHAR(5),@duration/3600)+'h:'+ CONVERT(VARCHAR(5),@duration%3600/60)+'m:' + CONVERT(VARCHAR(5),(@duration%60)) + 's'
RAISERROR ( '%s Day filled in: %s', 10, 0,@from1,@time ) WITH NOWAIT

END
------------------------------------------------------------------------------------------------------------------------------------------------------