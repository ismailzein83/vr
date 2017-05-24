-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dataparser].[sp_ParserType_Insert]
	@parserTypeId uniqueidentifier,
	@Name nvarchar(255),
		@Settings nvarchar(MAX)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM dataparser.[ParserType] WHERE Name = @Name)
	BEGIN
		INSERT INTO dataparser.[ParserType](ID,Name, Settings)
		VALUES (@parserTypeId,@Name, @Settings)

	END
 
END