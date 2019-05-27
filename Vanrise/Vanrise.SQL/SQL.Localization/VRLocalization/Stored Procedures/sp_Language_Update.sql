CREATE PROCEDURE [VRLocalization].[sp_Language_Update]
@ID uniqueidentifier,
@Name nvarchar(255),
@ParentLanguageID uniqueidentifier,
@Settings nvarchar(max)
AS
BEGIN
UPDATE	[VRLocalization].[Language] 
SET		Name = @Name, ParentLanguageID = @ParentLanguageID, Settings = @Settings
WHERE	ID = @ID;
END