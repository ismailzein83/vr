-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_VRDynamicAPIModule_Insert]
    @Id uniqueidentifier,
    @Name nvarchar(255),
	@DevProjectId uniqueidentifier,
	@CreatedBy int,
	@LastModifiedBy int
	
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM common.[VRDynamicAPIModule] where Name=@Name )
	BEGIN
	INSERT INTO common.VRDynamicAPIModule(Id, Name,DevProjectId,CreatedBy, LastModifiedTime,LastModifiedBy)
	VALUES (@Id, @Name,@DevProjectId,@CreatedBy, GETDATE(), @LastModifiedBy)
	
	END
END