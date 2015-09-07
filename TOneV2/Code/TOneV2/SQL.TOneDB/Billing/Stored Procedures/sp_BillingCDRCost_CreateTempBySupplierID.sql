-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Billing].[sp_BillingCDRCost_CreateTempBySupplierID]
	@TempTableName VARCHAR(200),
	@SupplierID VARCHAR(5),
	@From DATETIME,
	@To DATETIME
AS
BEGIN
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	BEGIN
		SET @From = DATEADD(DAY, 0, DATEDIFF(DAY, 0, @From))
		SET @To = DATEADD(S, -1, DATEADD(DAY, 1, DATEDIFF(DAY, 0, @To)))

		DECLARE @SupplierGMTTime INT
		SET @SupplierGMTTime = (SELECT GMTTime FROM dbo.CarrierAccount WHERE CarrierAccountID = @SupplierID);

		DECLARE @Result TABLE
		(
			[Day] DATETIME,
			DurationInMinutes NUMERIC(13, 4),
			Amount FLOAT,
			CurrencyID VARCHAR(3)
		)

		INSERT INTO @Result
		SELECT
			CONVERT(VARCHAR(10), DATEADD(MI, -@SupplierGMTTime, main.Attempt), 121) AS [Day],
			ISNULL((SUM(cost.DurationInSeconds) / 60.0), 0) AS DurationInMinutes,
			SUM(cost.Net) AS Amount,
			cost.CurrencyID
		FROM dbo.Billing_CDR_Main main WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt, IX_Billing_CDR_Main_Supplier))
		LEFT JOIN dbo.Billing_CDR_Cost cost ON main.ID = cost.ID
		WHERE main.SupplierID = @SupplierID
			AND main.Attempt >= @From
			AND main.Attempt <= @To
		GROUP BY CONVERT(VARCHAR(10), DATEADD(MI, -@SupplierGMTTime, main.Attempt), 121),
			cost.CurrencyID
		ORDER BY CONVERT(VARCHAR(10), DATEADD(MI, -@SupplierGMTTime, main.Attempt), 121)

		-- The insert operation is done

		SELECT
			CASE
				WHEN @SupplierGMTTime <= 0 THEN CONVERT(VARCHAR(10), [Day], 121)
				WHEN @SupplierGMTTime > 0 THEN CONVERT(VARCHAR(10), [Day] + 1, 121)
			END AS [Day],
			DurationInMinutes,
			Amount,
			CurrencyID
		
		INTO #RESULT
		
		FROM @Result

		ORDER BY [Day]
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
END