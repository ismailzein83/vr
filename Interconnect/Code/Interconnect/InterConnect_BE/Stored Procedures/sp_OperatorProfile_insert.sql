-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [InterConnect_BE].[sp_OperatorProfile_insert]
    @Name nvarchar(255),
	@Settings nvarchar(Max),
	@ExtendedSettingsRecordTypeId int = null,
	@ExtendedSettings nvarchar(Max)	,
	@ID int OUT
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM InterConnect_BE.OperatorProfile WHERE ID = @ID)
	BEGIN
		INSERT INTO  InterConnect_BE.OperatorProfile (Name,Settings,ExtendedSettingsRecordTypeID,ExtendedSettings)
		VALUES(@Name,@Settings,@ExtendedSettingsRecordTypeId,@ExtendedSettings)
		SET @ID = @@IDENTITY
	END
END