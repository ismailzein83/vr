CREATE PROCEDURE [VRLocalization].[sp_TextResource_Update]
@ID uniqueidentifier,
@DefaultValue nvarchar(max)
AS
BEGIN
UPDATE	[VRLocalization].[TextResource] 
SET		DefaultValue=@DefaultValue
WHERE	ID = @ID;
END