CREATE PROCEDURE [VRLocalization].[sp_Module_Update]
@ID uniqueidentifier,
@Name nvarchar(255)
AS
BEGIN
UPDATE	[VRLocalization].[Module] 
SET		Name = @Name
WHERE	ID = @ID;
END