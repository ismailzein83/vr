-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Analytics].[sp_Rate_GetByPriceListID]
	@TempTableName VARCHAR(200),
	@PriceListID INT
AS
BEGIN
	SET NOCOUNT ON;
		IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
				BEGIN
                        ;WITH 
                         MyZone AS 
                          (
	                        SELECT  z.Zoneid,
									z.Name,
									z.CodeGroup 
	                        FROM Zone z with(nolock)
                           )
                          ,
						  CTE_result AS 
						  ( 
						  SELECT r.RateID,
								 r.PriceListID,
								 r.ZoneID,
								 r.Rate,
								 r.OffPeakRate,
								 r.WeekendRate,
								 r.Change,
								 r.ServicesFlag,
								 r.BeginEffectiveDate,
								 r.EndEffectiveDate,
								 r.Notes
						  FROM	 rate r 
						  WHERE  PriceListID = @PriceListID
						  ) 
                        SELECT r.*,
							   z.Name AS ZoneName,
							   z.CodeGroup AS CodeGroup
						INTO
						#Result
                        FROM   CTE_result r
                        INNER JOIN MyZone z on z.zoneID= r.ZoneID
                  DECLARE @sql VARCHAR(1000)
				  SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
				  EXEC(@sql)
			END
                      
END