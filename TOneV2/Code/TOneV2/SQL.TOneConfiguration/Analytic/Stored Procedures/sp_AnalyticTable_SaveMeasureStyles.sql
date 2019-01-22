CREATE PROCEDURE [Analytic].[sp_AnalyticTable_SaveMeasureStyles]
	@ID uniqueidentifier,
	@MeasureStyles nvarchar(MAX)
AS
BEGIN
		Update Analytic.AnalyticTable
	Set MeasureStyles = @MeasureStyles, LastModifiedTime = GETDATE()
	Where ID = @ID
END