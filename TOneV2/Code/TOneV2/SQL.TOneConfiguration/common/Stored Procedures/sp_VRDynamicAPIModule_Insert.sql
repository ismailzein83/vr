-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_VRDynamicAPIModule_Insert]
    @Name nvarchar(255),
	@CreatedBy int,
	@LastModifiedBy int,
	@Id int OUT
AS
BEGIN
SET @Id =0;
IF NOT EXISTS(SELECT 1 FROM common.[VRDynamicAPIModule] where Name=@Name )
	BEGIN
	INSERT INTO common.VRDynamicAPIModule(Name,CreatedBy, LastModifiedTime,LastModifiedBy)
	VALUES (@Name,@CreatedBy, GETDATE(), @LastModifiedBy)
	SET @Id = SCOPE_IDENTITY()
	END
END