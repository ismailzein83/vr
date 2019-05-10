Create PROCEDURE [Analytic].[sp_AnalyticTable_SavePermanentFilter]
	@ID uniqueidentifier,
	@PermanentFilter nvarchar(MAX)
AS
BEGIN
		Update Analytic.AnalyticTable
	Set PermanentFilter = @PermanentFilter, LastModifiedTime = GETDATE()
	Where ID = @ID
END