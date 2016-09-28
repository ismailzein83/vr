-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_BusinessEntity_Insert]
	-- Add the parameters for the stored procedure here
	@Id uniqueidentifier,
	@Name nvarchar(255),
	@Title  nvarchar(255),
	@ModuleId uniqueidentifier,
	@BreakInheritance bit,
	@PermissionOptions varchar(255)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
IF NOT EXISTS(select 1 from sec.BusinessEntity where Name = @Name)
	BEGIN	
		INSERT INTO sec.BusinessEntity(ID,Name,Title,ModuleId,BreakInheritance,PermissionOptions) 
		VALUES (@Id,@Name,@Title,@ModuleId,@BreakInheritance,@PermissionOptions)
	 -- Insert statements for procedure here
	END
END