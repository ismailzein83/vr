-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_NormalizationRule_CreateTempByFiltered]
	@TempTableName VARCHAR(200) = NULL,
	@EffectiveDate DATETIME = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
    BEGIN
		SELECT ID, Criteria, Settings, [Description], BED, EED
		
		INTO #RESULT
		
		FROM PSTN_BE.NormalizationRule
		
		WHERE (@EffectiveDate IS NULL OR (BED <= @EffectiveDate AND EED >= @EffectiveDate))
			
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
    END
	
    SET NOCOUNT OFF;
END