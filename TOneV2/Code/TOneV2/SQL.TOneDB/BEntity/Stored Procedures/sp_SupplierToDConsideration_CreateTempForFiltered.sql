

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_SupplierToDConsideration_CreateTempForFiltered]
	@TempTableName varchar(200),
	@zoneIds varchar(max),
	@SupplierAMUIDs varchar(max),
	@supplierId varchar(10)=null,
	@effectiveOn datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			
		DECLARE @ZoneIdsTable TABLE (ZoneId int)
		INSERT INTO @ZoneIdsTable (ZoneId)
		select Convert(int, ParsedString) from [BEntity].[ParseStringList](@zoneIds)
		
		DECLARE @SupplierIDs TABLE (SupplierId varchar(10))
		INSERT INTO @SupplierIDs (SupplierId)
		select  ParsedString  from [BEntity].[ParseStringList](@SupplierAMUIDs)
		
		
	    DECLARE @ShowNameSuffix nvarchar(1)
        SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like 'ShowNameSuffix')
        ;with CarrierTables as                          
          (SELECT
			(CASE
			 WHEN  @ShowNameSuffix ='Y'
			 THEN 
				(CASE
				  when A.NameSuffix!='' THEN 
				    P.Name+'('+A.NameSuffix+')' 
				  ELSE 
				    P.Name
				 END) 
			  ELSE 
				(P.Name )
			   END )
			  AS CarrierName
             ,A.CarrierAccountID as CarrierID  from CarrierAccount A inner join CarrierProfile P on P.ProfileID=A.ProfileID
            )
		  ,MyZone AS 
			(SELECT  z.Zoneid,z.Name,z.CodeGroup 
			  FROM Zone z with(nolock)
			  where z.isEffective= 'Y'
			)
            SELECT 
             tod.ToDConsiderationID 
             ,tod.ZoneID
             ,tod.SupplierID
             ,tod.CustomerID
             ,tod.BeginTime
             ,tod.EndTime
             ,tod.WeekDay
             ,tod.HolidayDate
             ,tod.HolidayName  
             ,tod.RateType
             ,tod.BeginEffectiveDate
             ,tod.EndEffectiveDate
             ,tod.UserID 
		     ,u.Name as UserName
             ,ca.CarrierName as CarrierName
             ,ca2.CarrierName as customerName
             ,Z.Name as ZoneName
             ,CASE WeekDay
				 WHEN NULL THEN 'Everyday'
				 WHEN 0 THEN 'Sunday'
				 WHEN 1 THEN 'Monday'
				 WHEN 2 THEN 'Tuesday'
				 WHEN 3 THEN 'Wednesday'
				 WHEN 4 THEN 'Thursday'
				 WHEN 5 THEN 'Friday'
				 ELSE 'Saturday'
				END  AS WeekDayName
		    ,( CASE RateType
				WHEN 1 THEN 'OffPeak (' +   CASE WeekDay
										 WHEN NULL THEN 'Everyday'
										 WHEN 0 THEN 'Sunday'
										 WHEN 1 THEN 'Monday'
										 WHEN 2 THEN 'Tuesday'
										 WHEN 3 THEN 'Wednesday'
										 WHEN 4 THEN 'Thursday'
										 WHEN 5 THEN 'Friday'
										 ELSE 'Saturday'
										 END + 
				' From ' + beginTime + ' To ' + EndTime +' On ' + z.name +')' 
				WHEN 2 THEN 'weekend (' +   CASE WeekDay
										 WHEN NULL THEN 'Everyday'
										 WHEN 0 THEN 'Sunday'
										 WHEN 1 THEN 'Monday'
										 WHEN 2 THEN 'Tuesday'
										 WHEN 3 THEN 'Wednesday'
										 WHEN 4 THEN 'Thursday'
										 WHEN 5 THEN 'Friday'
										 ELSE 'Saturday'
										 END + 
				' From ' + beginTime + ' To ' + EndTime +' on ' + z.name +')' 
									
				WHEN 4 THEN  'Holiday ( ' + HolidayName + '-'+  DATENAME(month, HolidayDate) + CONVERT(varchar, DATEPART(day, HolidayDate)) + ' on '+ z.Name + ')'
				WHEN 0 THEN  ' ' 
				END
			) as DefinitionDisplayS
            INTO #RESULT
            FROM ToDConsideration tod
            LEFT JOIN  [user] u ON u.ID = tod.UserID
            LEFT JOIN CarrierTables ca ON ca.CarrierID = tod.SupplierID
            LEFT JOIN CarrierTables ca2 ON ca2.CarrierID = tod.CustomerID
            JOIN MyZone Z ON Z.ZoneID = tod.ZoneID
            WHERE (@SupplierAMUIDs IS NULL OR tod.SupplierID IN (SELECT * FROM @SupplierIDs))  
			AND TOD.CustomerID='SYS'
			AND ( @supplierId is Null or tod.SupplierID = @supplierId  )
            AND ( @zoneIds is Null or tod.ZoneID IN (SELECT ZoneID FROM @ZoneIdsTable)  )
			AND TOD.BeginEffectiveDate <= @effectiveOn  AND ( TOD.EndEffectiveDate IS NULL OR TOD.EndEffectiveDate > @effectiveOn)   
            ORDER BY tod.BeginEffectiveDate Desc
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END

END