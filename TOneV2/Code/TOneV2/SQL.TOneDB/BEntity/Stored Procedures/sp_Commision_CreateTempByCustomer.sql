-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Commision_CreateTempByCustomer]
	@TempTableName varchar(200),
	@CustomerID 	varchar(10),
	@ZoneIds varchar(max)  ,
	@EffectiveFrom DateTime =NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	 IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
	     IF(@ZoneIds IS NOT NULL)
				BEGIN
				DECLARE @CustomerZoneIdsTable TABLE (ZoneId int)
				INSERT INTO @CustomerZoneIdsTable (ZoneId)
				select Convert(int, ParsedString) from [BEntity].[ParseStringList](@ZoneIds)
			END
	      ;with 
                    MyZone AS 
						(
						  SELECT  z.Zoneid,z.Name,z.CodeGroup 
						  FROM Zone z with(nolock)
                          where z.isEffective= 'Y'
	                    )
                        , CarrierTables as                      
                        (
                            SELECT
                          ( case when A.NameSuffix!='' THEN  P.Name+'('+A.NameSuffix+')' else P.Name end ) AS CarrierName
                            ,A.CarrierAccountID as CarrierID,p.CurrencyID AS currency  FROM CarrierAccount A INNER JOIN CarrierProfile P ON P.ProfileID=A.ProfileID
                          )
                         SELECT C.Amount,
								c.BeginEffectiveDate,
								c.CommissionID,
								c.CustomerID,
								c.EndEffectiveDate,
								c.FromRate,
								c.IsEffective,
								c.IsExtraCharge,
								c.Percentage,
								c.SupplierID,
								c.ToRate,
								c.UserID,
								c.ZoneID,
								z.Name AS zoneName,
								ca.CarrierName AS CustomerName ,
								ca2.currency AS Currency
                         INTO #RESULT 
                         FROM Commission c
                         JOIN MyZone z ON z.zoneid=c.zoneid
                         LEFT JOIN CarrierTables ca ON ca.CarrierID = c.CustomerID
                         LEFT JOIN CarrierTables ca2 ON ca2.CarrierID = c.SupplierID
                         WHERE c.SupplierID='SYS'   
								and(@CustomerID IS NULL or c.CustomerID=@CustomerID)
								 AND((@ZoneIds IS NULL)Or (c.ZoneID IN(SELECT ZoneId from @CustomerZoneIdsTable)))
								AND (@EffectiveFrom IS NULL or (c.BeginEffectiveDate<= @EffectiveFrom AND (c.EndEffectiveDate IS NULL OR  c.EndEffectiveDate > @EffectiveFrom))) 
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END
		
		
END