-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Tariff_CreateTempBySupplierID]
	@TempTableName VARCHAR(200),
	@SelectedSupplierID VARCHAR(5),
	@SelectedZoneIDs VARCHAR(MAX),
	@EffectiveOn DATETIME
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
    BEGIN
		DECLARE @ZoneIDsTable TABLE (ZoneID INT);
		INSERT INTO @ZoneIDsTable (ZoneID)
		SELECT CONVERT(INT, ParsedString) FROM [BEntity].[ParseStringList](@SelectedZoneIDs);
    
		WITH CarriersCTE AS
		(
			SELECT ca.CarrierAccountID, cp.Name AS SupplierName, ca.NameSuffix AS SupplierNameSuffix, ca.GMTTime, cp.CurrencyID
			FROM CarrierAccount ca INNER JOIN CarrierProfile cp ON cp.ProfileID = ca.ProfileID
			--WHERE ca.ActivationStatus = 2
		),

		ZonesCTE AS (SELECT ZoneID, Name AS ZoneName FROM Zone WHERE IsEffective = 'Y')

		SELECT
			t.TariffID,
			t.ZoneID,
			z.ZoneName,
			t.SupplierID,
			c.SupplierName,
			c.SupplierNameSuffix,
			c.CurrencyID,
			t.CallFee,
			t.FirstPeriod,
			t.FirstPeriodRate,
			t.FractionUnit,
			t.BeginEffectiveDate,
			t.EndEffectiveDate,
			t.IsEffective

		INTO #RESULT

		FROM Tariff t
		INNER JOIN CarriersCTE c ON c.CarrierAccountID = t.SupplierID
		INNER JOIN ZonesCTE z ON z.ZoneID = t.ZoneID
	    
		WHERE t.CustomerID = 'SYS'
			AND (@SelectedSupplierID IS NULL OR t.SupplierID = @SelectedSupplierID)
			AND (@SelectedZoneIDs IS NULL OR t.ZoneID IN (SELECT ZoneID FROM @ZoneIDsTable))
			AND t.BeginEffectiveDate <= DATEADD(HH, c.GMTTime, @EffectiveOn)
			AND (t.EndEffectiveDate IS NULL OR t.EndEffectiveDate > DATEADD(HH, c.GMTTime, @EffectiveOn))

		ORDER BY t.BeginEffectiveDate
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
    END
END