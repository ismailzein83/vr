CREATE PROCEDURE [QM_BE].[sp_Zone_InsertFromSource]
	@ID int,
	@Name nvarchar(255),
	@SourceZoneID varchar(255),
	@CountryID int,
	@BED datetime,
	@Settings nvarchar(MAX),
	@IsFromTestingConnectorZone bit
	
AS

BEGIN
	Insert into QM_BE.Zone([ID],[Name],[SourceZoneID],[CountryID],[BED],[Settings],[IsFromTestingConnectorZone])
	Values(@ID,@Name,@SourceZoneID,@CountryID ,@BED,@Settings, @IsFromTestingConnectorZone)
END