CREATE PROCEDURE [VRLocalization].[sp_Language_Insert]
@ID uniqueidentifier,
@Name nvarchar(255),
@ParentLanguageID uniqueidentifier,
@Settings nvarchar(max)
AS
BEGIN
Insert into [VRLocalization].[Language](ID,Name,ParentLanguageID,Settings) 
values (@ID,@Name,@ParentLanguageID,@Settings);
END