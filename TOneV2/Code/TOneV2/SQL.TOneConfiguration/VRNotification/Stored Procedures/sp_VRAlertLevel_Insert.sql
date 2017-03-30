-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VRNotification].[sp_VRAlertLevel_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@BusinessEntityDefinitionId uniqueidentifier,
    @Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM [VRNotification].VRAlertLevel WHERE Name = @Name and BusinessEntityDefinitionID = @BusinessEntityDefinitionId)
	BEGIN
	INSERT INTO [VRNotification].[VRAlertLevel] (ID,Name,BusinessEntityDefinitionID,Settings)
	VALUES (@ID, @Name,@BusinessEntityDefinitionId,@Settings)
	END
END