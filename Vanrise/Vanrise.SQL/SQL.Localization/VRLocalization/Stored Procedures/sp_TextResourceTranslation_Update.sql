CREATE PROCEDURE [VRLocalization].[sp_TextResourceTranslation_Update]
@ID uniqueidentifier,
@TextResourceID uniqueidentifier,
@LanguageID uniqueidentifier,
@Settings nvarchar(max)
AS
BEGIN
IF NOT EXISTS(Select 1 from  [VRLocalization].[TextResourceTranslation] WITH(NOLOCK) where TextResourceID = @TextResourceID and LanguageID=@LanguageID and ID != @ID)
BEGIN
update	[VRLocalization].[TextResourceTranslation] 
SET		TextResourceID = @TextResourceID, LanguageID = @LanguageID,Settings=@Settings
where	ID = @ID
END
END