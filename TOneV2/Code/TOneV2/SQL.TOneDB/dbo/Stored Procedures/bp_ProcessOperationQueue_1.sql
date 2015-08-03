CREATE PROCEDURE [dbo].[bp_ProcessOperationQueue]
AS
BEGIN
	DECLARE @OperationQueue TABLE (Type char(1) NOT NULL, ID bigint NOT NULL, Operation char(1) NOT NULL)
	
	INSERT INTO @OperationQueue	SELECT Type, ID, Operation FROM OperationQueue
	
	TRUNCATE TABLE OperationQueue
	
	DECLARE @Type char(1)
	DECLARE @ID bigint
	DECLARE @Operation char(1)
	
	DECLARE @EndFlag char(1)
	DECLARE @BeginFlag char(1) 
	SET @BeginFlag = 'B'
	SET @EndFlag = 'E'	

	DECLARE OperationCur CURSOR LOCAL FORWARD_ONLY
		FOR SELECT [Type], ID, Operation FROM @OperationQueue 
	OPEN OperationCur
	FETCH NEXT FROM OperationCur INTO @Type, @ID, @Operation
	WHILE @@FETCH_STATUS <> 0 
	BEGIN
		
		FETCH NEXT FROM OperationCur INTO @Type, @ID, @Operation		
	END
		
END