-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE logging.sp_LoggableEntity_InsertOrUpdate 
	@UniqueName varchar(255),
	@Settings nvarchar(max)
AS
BEGIN
	DECLARE @ID INT = (SELECT ID FROM [logging].[LoggableEntity] WITH(NOLOCK) WHERE [UniqueName] = @UniqueName)
	
	IF @ID IS NULL
	BEGIN
		INSERT INTO [logging].[LoggableEntity]
		([UniqueName], [Settings])
		SELECT @UniqueName, @Settings WHERE NOT EXISTS (SELECT TOP 1 NULL FROM [logging].[LoggableEntity] WHERE [UniqueName] = @UniqueName)

		SET @ID = (SELECT ID FROM [logging].[LoggableEntity] WITH(NOLOCK) WHERE [UniqueName] = @UniqueName)
	END

	UPDATE [logging].[LoggableEntity]
	SET Settings = @Settings
	WHERE ID = @ID
	
	SELECT @ID
END