-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_BusinessEntity_Update]
	-- Add the parameters for the stored procedure here
	@Id uniqueidentifier ,
	@Name NVARCHAR(255),
	@Title  nvarchar(255),
	@ModuleId uniqueidentifier,
	@BreakInheritance bit,
	@PermissionOptions varchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
IF NOT EXISTS(select 1 from sec.[BusinessEntity] where Name = @Name and Id!=@Id)
	BEGIN		
	UPDATE	sec.[BusinessEntity]
		SET		Name = @Name,
				Title = @Title,
				ModuleId=@ModuleId,
				BreakInheritance = @BreakInheritance,
				PermissionOptions=@PermissionOptions
		WHERE	Id = @Id
	END
END