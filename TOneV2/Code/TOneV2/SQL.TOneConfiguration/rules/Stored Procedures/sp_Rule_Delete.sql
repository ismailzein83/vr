-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [rules].[sp_Rule_Delete] 
	@ID INT
AS
BEGIN
	DECLARE @TypeId int
	SELECT @TypeId = TypeID FROM [rules].[Rule] where ID = @ID
	
	DELETE FROM [rules].[Rule] WHERE ID=@ID
	
	IF @@ROWCOUNT > 0
	BEGIN
		--this query increment the timestamp column in order to clears the caches in the application
		DECLARE @FirstID int
		SELECT TOP 1 @FirstID = ID FROM [rules].[Rule] where TypeID = @TypeId
		UPDATE [rules].[Rule] 
		SET TypeID = TypeID
		WHERE ID = @FirstID
	END

END