-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_VRDynamicAPIModule_Update]
    @ID INT,
    @Name nvarchar(255),
	@LastModifiedBy int

AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM common.[VRDynamicAPIModule] WHERE ID != @ID AND Name = @Name)
	BEGIN
	UPDATE common.VRDynamicAPIModule
		SET Name=@Name , LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
	WHERE ID = @ID
	END
END