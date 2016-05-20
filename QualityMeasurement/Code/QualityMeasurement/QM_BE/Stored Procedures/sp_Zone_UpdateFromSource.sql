CREATE PROCEDURE [QM_BE].[sp_Zone_UpdateFromSource]
	@ID int,
	@Name nvarchar(255),
	@SourceZoneID varchar(255),
	@CountryID int,
	@BED datetime,
	@EED datetime,
	@Settings nvarchar(MAX),
	@IsFromTestingConnectorZone bit
AS
BEGIN

	Update QM_BE.Zone
	Set
	 Name = @Name,
	 SourceZoneID = @SourceZoneID,
	 CountryID = @CountryID,
	 BED = @BED,
	 EED = @EED,
	 Settings = @Settings,
	 IsFromTestingConnectorZone = @IsFromTestingConnectorZone
	Where ID = @ID

END