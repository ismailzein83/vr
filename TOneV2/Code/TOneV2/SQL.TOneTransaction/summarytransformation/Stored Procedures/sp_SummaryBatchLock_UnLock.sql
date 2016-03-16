CREATE PROCEDURE [summarytransformation].[sp_SummaryBatchLock_UnLock]
	@TypeID int,
	@BatchStart datetime
AS
BEGIN
	UPDATE summarytransformation.SummaryBatchLock
    SET	
			LockedByProcessID = NULL
	WHERE TypeID = @TypeID 
			AND BatchStart = @BatchStart
END