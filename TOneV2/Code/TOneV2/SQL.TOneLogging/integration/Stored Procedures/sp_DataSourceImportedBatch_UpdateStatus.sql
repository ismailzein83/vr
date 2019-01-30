-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSourceImportedBatch_UpdateStatus] 
	@BatchStatusToUpdate DataSourceImportedBatchExecutionStatusType readonly
AS
BEGIN
	UPDATE importedBatches 
	set importedBatches.ExecutionStatus = newBatches.ExecutionStatus
	FROM [integration].[DataSourceImportedBatch] importedBatches with(nolock)
	JOIN @BatchStatusToUpdate newBatches on newBatches.ID = importedBatches.ID
END