-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Rate_CreateTempByFiltered]
	@TempTableName varchar(200),
	@ZoneId INT,
	@EffectedDate SMALLDATETIME =NULL,
	@CustomerId VARCHAR(5)= NULL,
	@SupplierId VARCHAR(5)= NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			SELECT  R.RateID, 
					R.Rate, 
					R.OffPeakRate, 
					R.WeekendRate, 
					R.Change,
					p.CurrencyID, 
					R.ServicesFlag,
					R.BeginEffectiveDate, 
					R.EndEffectiveDate,
					R.Notes, 
					R.IsEffective,
					R.PriceListID
					
					
			INTO #RESULT
            FROM Rate R
            LEFT JOIN PriceList P ON P.PriceListID = R.PriceListID
            LEFT JOIN ZONE Z ON Z.ZoneID = R.ZoneID
            
            WHERE R.ZoneID = @ZoneId
				AND (@EffectedDate IS NULL OR @EffectedDate<= R.BeginEffectiveDate)
				AND (P.CustomerID = @CustomerId OR @CustomerId IS NULL)
				AND (P.SupplierID = @SupplierId OR @SupplierId IS NULL)
            ORDER BY R.BeginEffectiveDate DESC ,r.RateID DESC
            
            DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
	END
END