CREATE PROCEDURE [sec].[sp_OrgChartLinkedEntity_Insert]
	@OrgChartID int,
	@LinkedEntityIdentifier varchar(850)
AS
BEGIN
	IF Exists (Select 1 from sec.OrgChartLinkedEntity where LinkedEntityIdentifier = @LinkedEntityIdentifier)
		BEGIN
			UPDATE sec.OrgChartLinkedEntity 
			SET OrgChartID = @OrgChartID
			Where LinkedEntityIdentifier = @LinkedEntityIdentifier
			
		END
	ELSE
		BEGIN
			INSERT INTO sec.OrgChartLinkedEntity (OrgChartID, LinkedEntityIdentifier)
			VALUES (@OrgChartID, @LinkedEntityIdentifier)
		END		
END