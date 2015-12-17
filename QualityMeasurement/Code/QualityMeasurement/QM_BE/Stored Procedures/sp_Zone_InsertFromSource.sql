CREATE PROCEDURE [QM_BE].[sp_Zone_InsertFromSource]
	@ID int,
	@Name nvarchar(255),
	@SourceZoneID varchar(255),
	@CountryID int,
	@BED datetime
AS

BEGIN
	Insert into QM_BE.Zone([ID],[Name],[SourceZoneID],[CountryID],[BED])
	Values(@ID,@Name,@SourceZoneID,@CountryID ,@BED)
END