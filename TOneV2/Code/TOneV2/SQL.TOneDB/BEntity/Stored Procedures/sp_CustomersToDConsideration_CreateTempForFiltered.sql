

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_CustomersToDConsideration_CreateTempForFiltered]
	@TempTableName varchar(200),
	@zoneIds varchar(max),
	@customerId varchar(10)=null,
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

		
			SELECT
				  td.ToDConsiderationID 
                 ,td.ZoneID
                 ,td.SupplierID
                 ,td.CustomerID
                 ,td.BeginTime
                 ,td.EndTime
                 ,td.WeekDay
                 ,td.HolidayDate
                 ,td.HolidayName  
                 ,td.RateType
                 ,td.BeginEffectiveDate
                 ,td.EndEffectiveDate
                 ,td.UserID
                 ,z.name as ZoneName
                 ,c.NameSuffix AS CustomerNameSuffix
                 ,cp.Name AS CarrierName
                 ,( CASE RateType
					WHEN 0 THEN 'Normal'
					WHEN 1 THEN 'OffPeak'
					WHEN 2 THEN 'weekend'
					WHEN 4 THEN 'Holiday'
					END	
					+'('+
					CASE WeekDay
					WHEN NULL THEN 'Everyday'
					WHEN 0 THEN 'Sunday'
					WHEN 1 THEN 'Monday'
					WHEN 2 THEN 'Tuesday'
					WHEN 3 THEN 'Wednesday'
					WHEN 4 THEN 'Thursday'
					WHEN 5 THEN 'Friday'
					ELSE 'Saturday'
					END 
					+ ' From ' + beginTime + ' To ' + EndTime +' On ' + z.name +')' 
				   )
					as DefinitionDisplayS
			INTO #RESULT
			FROM ToDConsideration td 
            LEFT JOIN Zone z on z.ZoneID = td.ZoneID
            LEFT JOIN CarrierAccount c on c.CarrierAccountID=td.CustomerID
            LEFT JOIN CarrierProfile cp on cp.ProfileID=c.ProfileID                                 
            WHERE  
				c.IsDeleted='N' 
				AND td.SupplierID = 'SYS'
				AND ((td.BeginEffectiveDate <= @effectiveOn AND (td.EndEffectiveDate IS NULL OR td.EndEffectiveDate > @effectiveOn)) OR td.BeginEffectiveDate > @effectiveOn)
                AND ( @customerId is Null or td.CustomerID = @customerId  )
                AND ( @zoneIds is Null or td.ZoneID IN (SELECT ZoneID FROM @ZoneIdsTable)  )  
            ORDER BY td.BeginEffectiveDate Desc
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END

END