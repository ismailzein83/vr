-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_VRDynamicAPI_Insert]
    @Name nvarchar(255),
	@ModuleId int,
	@Settings nvarchar(max),
	@CreatedBy int,
	@LastModifiedBy int,
	@Id bigint OUT
AS
BEGIN
SET @Id =0;
IF NOT EXISTS(SELECT 1 FROM common.[VRDynamicAPI] where Name=@Name AND ModuleId = @ModuleId )
	BEGIN
	INSERT INTO common.VRDynamicAPI(Name,ModuleId,Settings,CreatedBy,LastModifiedTime,LastModifiedBy)
	VALUES (@Name,@ModuleId,@Settings,@CreatedBy, GETDATE(), @LastModifiedBy)
	SET @Id = SCOPE_IDENTITY()
	END
END