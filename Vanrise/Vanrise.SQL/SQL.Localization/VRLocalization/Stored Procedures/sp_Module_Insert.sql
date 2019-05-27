CREATE PROCEDURE [VRLocalization].[sp_Module_Insert]
@ID uniqueidentifier,
@Name nvarchar(255)
AS
BEGIN
Insert into [VRLocalization].[Module](ID,Name) 
values (@ID,@Name);
END