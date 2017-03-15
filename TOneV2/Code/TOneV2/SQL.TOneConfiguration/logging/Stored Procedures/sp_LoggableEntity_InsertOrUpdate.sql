-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [logging].[sp_LoggableEntity_InsertOrUpdate] 
	@UniqueName varchar(255),
	@Settings nvarchar(max)
AS
BEGIN
	DECLARE @ID uniqueidentifier = (SELECT ID FROM [logging].[LoggableEntity] WITH(NOLOCK) WHERE [UniqueName] = @UniqueName)
	
	IF @ID IS NULL
		BEGIN
			INSERT INTO [logging].[LoggableEntity]
			(ID, [UniqueName], [Settings])
			SELECT newid(), @UniqueName, @Settings WHERE NOT EXISTS (SELECT TOP 1 NULL FROM [logging].[LoggableEntity] WHERE [UniqueName] = @UniqueName)

			SET @ID = (SELECT ID FROM [logging].[LoggableEntity] WITH(NOLOCK) WHERE [UniqueName] = @UniqueName)
		END
	ELSE
		BEGIN
			UPDATE	[logging].[LoggableEntity]
			SET		Settings = @Settings
			WHERE	ID = @ID
		END

	SELECT @ID
END