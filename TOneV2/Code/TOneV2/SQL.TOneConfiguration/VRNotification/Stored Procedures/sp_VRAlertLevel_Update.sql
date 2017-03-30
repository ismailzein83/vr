-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VRNotification].[sp_VRAlertLevel_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@BusinessEntityDefinitionId uniqueidentifier,
	@Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM [VRNotification].VRAlertLevel WHERE ID != @ID and Name = @Name and BusinessEntityDefinitionID = @BusinessEntityDefinitionId)
	BEGIN
		update [VRNotification].VRAlertLevel
		set  Name = @Name ,Settings= @Settings, BusinessEntityDefinitionID = @BusinessEntityDefinitionId
		where  ID = @ID
	END
END