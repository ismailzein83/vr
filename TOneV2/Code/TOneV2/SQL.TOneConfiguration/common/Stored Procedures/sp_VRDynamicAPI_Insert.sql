-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_VRDynamicAPI_Insert]
    @Id uniqueidentifier,
    @Name nvarchar(255),
	@ModuleId uniqueidentifier,
	@Settings nvarchar(max),
	@CreatedBy int,
	@LastModifiedBy int
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM common.[VRDynamicAPI] where Name=@Name AND ModuleId = @ModuleId )
	BEGIN
	INSERT INTO common.VRDynamicAPI(Name,ModuleId,Settings,CreatedBy,LastModifiedTime,LastModifiedBy)
	VALUES (@Name,@ModuleId,@Settings,@CreatedBy, GETDATE(), @LastModifiedBy)
	END
END