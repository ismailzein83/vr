-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [InterConnect_BE].[sp_OperatorProfile_Update]
    @ID INT,
    @Name nvarchar(255),
	@Settings nvarchar(Max),
	@ExtendedSettingsRecordTypeId int = null,
	@ExtendedSettings nvarchar(Max)	
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM InterConnect_BE.OperatorProfile WHERE ID != @ID AND Name = @Name)
	BEGIN
		UPDATE InterConnect_BE.OperatorProfile
		SET Name=@Name , 
			Settings = @Settings,
			ExtendedSettingsRecordTypeID = @ExtendedSettingsRecordTypeId,
			ExtendedSettings = @ExtendedSettings
			
		WHERE ID = @ID
	END
END