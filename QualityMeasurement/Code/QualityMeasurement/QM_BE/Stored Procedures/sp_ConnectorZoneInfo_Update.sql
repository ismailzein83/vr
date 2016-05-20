CREATE PROCEDURE [QM_BE].[sp_ConnectorZoneInfo_Update]
	@ID int,
	@Codes nvarchar(MAX)
AS
BEGIN

SELECT 1 FROM QM_BE.ConnectorZoneInfo WHERE ID = @Id
	BEGIN
		Update QM_BE.ConnectorZoneInfo
		Set 
			Codes = @Codes
	Where ID = @ID
	END	
END