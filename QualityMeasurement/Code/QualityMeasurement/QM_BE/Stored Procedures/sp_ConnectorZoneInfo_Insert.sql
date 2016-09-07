CREATE PROCEDURE [QM_BE].[sp_ConnectorZoneInfo_Insert]
	@ConnectorType varchar(50),
	@ConnectorZoneID varchar(50),
	@Codes nvarchar(MAX),
	@ID int out
AS

BEGIN
	Insert into QM_BE.ConnectorZoneInfo([ConnectorType],[ConnectorZoneID],[Codes],[CreatedTime])
	Values(@ConnectorType,@ConnectorZoneID,@Codes,GETDATE())
	
	Set @ID = SCOPE_IDENTITY()
END