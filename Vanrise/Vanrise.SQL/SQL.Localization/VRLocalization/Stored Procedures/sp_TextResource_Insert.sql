CREATE PROCEDURE [VRLocalization].[sp_TextResource_Insert]
@ID uniqueidentifier,
@ResourceKey varchar(255),
@ModuleID uniqueidentifier,
@Settings nvarchar(max)
AS
BEGIN
Insert into [VRLocalization].[TextResource](ID,ResourceKey,ModuleID,Settings) 
values (@ID,@ResourceKey,@ModuleID,@Settings);
END