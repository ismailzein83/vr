create PROCEDURE common.sp_GenericConfiguration_Update
	@OwnerKey varchar(50),
	@TypeID int, 
	@ConfigDetails varchar(MAX)
AS
BEGIN
    IF NOT EXISTS(SELECT * FROM [common].GenericConfiguration WHERE OwnerKey = @OwnerKey AND TypeID = @TypeID)
    BEGIN
		INSERT INTO [common].GenericConfiguration (OwnerKey,TypeID,ConfigDetails) VALUES (@OwnerKey,@TypeID,@ConfigDetails)
    END
    ELSE
    BEGIN
        UPDATE [common].GenericConfiguration
        SET ConfigDetails = @ConfigDetails
        WHERE (OwnerKey = @OwnerKey) AND (TypeID = @TypeID)
    END
END