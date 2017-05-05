-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_OverriddenConfigurationGroup_Insert]
@ID uniqueidentifier,
	@Name NVARCHAR(255)
	AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM [common].OverriddenConfigurationGroup WHERE Name = @Name)
	BEGIN
	INSERT INTO [common].OverriddenConfigurationGroup (ID,Name)
	VALUES (@ID, @Name)
	END
END