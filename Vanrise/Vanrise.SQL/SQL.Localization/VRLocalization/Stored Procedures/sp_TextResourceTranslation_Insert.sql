CREATE PROCEDURE [VRLocalization].[sp_TextResourceTranslation_Insert]
@ID uniqueidentifier,
@TextResourceID uniqueidentifier,
@LanguageID uniqueidentifier,
@Settings nvarchar(max)
AS
BEGIN
IF NOT EXISTS(Select 1 from  [VRLocalization].[TextResourceTranslation] WITH(NOLOCK) where TextResourceID = @TextResourceID and LanguageID=@LanguageID)
BEGIN
Insert into [VRLocalization].[TextResourceTranslation](ID,TextResourceID,LanguageID,Settings) 
values (@ID,@TextResourceID,@LanguageID,@Settings);
END
END