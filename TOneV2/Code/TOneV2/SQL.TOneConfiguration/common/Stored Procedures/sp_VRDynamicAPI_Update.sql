-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_VRDynamicAPI_Update]
    @ID BIGINT,
    @Name nvarchar(255),
	@ModuleId int,
	@Settings nvarchar(max),
	@LastModifiedBy int

AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM common.[VRDynamicAPI] WHERE ID != @ID AND Name = @Name and ModuleId = @ModuleId)
	BEGIN
	UPDATE common.VRDynamicAPI
		SET Name=@Name ,ModuleId=@ModuleId,Settings=@Settings, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
	WHERE ID = @ID
	END
END