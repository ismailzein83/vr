-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_BigDataService_DeleteTimedOut]
	@NotInRuntimeProcessIds varchar(max)
AS
BEGIN
	DECLARE @RuntimeProcessIDsTable TABLE (ProcessID int)
	INSERT INTO @RuntimeProcessIDsTable (ProcessID)
	select Convert(int, ParsedString) from [common].[ParseStringList](@NotInRuntimeProcessIds)
	
	DELETE FROM common.BigDataService
	WHERE RuntimeProcessID NOT IN (SELECT ProcessID FROM @RuntimeProcessIDsTable)
	
	IF @@ROWCOUNT > 0
	BEGIN
		--this query increment the timestamp column in order to clears the caches in the application
		DECLARE @FirstID bigint
		SELECT TOP 1 @FirstID = ID FROM common.BigDataService
		UPDATE common.BigDataService 
		SET ServiceURL = ServiceURL
		WHERE ID = @FirstID
	END
END