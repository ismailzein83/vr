-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Commision_CreateTempBySupplier]
	@TempTableName varchar(200),
	@SupplierID 	varchar(10),
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
				DECLARE @SupplierZoneIdsTable TABLE (ZoneId int)
				INSERT INTO @SupplierZoneIdsTable (ZoneId)
				select Convert(int, ParsedString) from [BEntity].[ParseStringList](@ZoneIds)
			END
                ;WITH
                MyZones as 
                (
					SELECT Z.Name,Z.ZoneID,z.CodeGroup  FROM ZONE Z with(nolock) WHERE Z.IsEffective = 'Y'
                ) 
                ,CarrierTables as
                (SELECT
                 (case when A.NameSuffix!='' THEN  P.Name+'('+A.NameSuffix+')' else P.Name end ) AS CarrierName
                   ,A.CarrierAccountID as CarrierID, a.IsDeleted , p.CurrencyID as Currency  from CarrierAccount A inner join CarrierProfile P on P.ProfileID=A.ProfileID
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
						C1.CarrierName AS SupplierName ,
						Z.Name AS ZoneName,
						c1.currency AS Currency
                   INTO			#RESULT
                   FROM			Commission C
                   LEFT JOIN	CarrierTables C1 ON C1.CarrierID = C.SupplierID
                   JOIN			MyZones Z ON Z.ZoneID = C.ZoneID
                   WHERE		c.SupplierID<>'SYS' 
                   AND			(@SupplierID IS NULL OR C.SupplierID =@SupplierID) 
                   AND((@ZoneIds IS NULL)Or (c.ZoneID IN(SELECT ZoneId from @SupplierZoneIdsTable)))
                   AND(@EffectiveFrom IS NULL or (c.BeginEffectiveDate<= @EffectiveFrom AND (c.EndEffectiveDate IS NULL OR  c.EndEffectiveDate > @EffectiveFrom))) 

							
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END
		
		
END