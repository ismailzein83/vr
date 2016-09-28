-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_BusinessEntityModule_Update]
	-- Add the parameters for the stored procedure here
	@Id uniqueidentifier ,
	@Name NVARCHAR(255),
	@ParentId uniqueidentifier,
	@BreakInheritance bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
IF NOT EXISTS(select 1 from sec.[BusinessEntityModule] where Name = @Name and Id!=@Id)
	BEGIN		
	UPDATE	sec.[BusinessEntityModule]
		SET		Name = @Name,
				ParentId=@ParentId,
				BreakInheritance = @BreakInheritance
		WHERE	Id = @Id
	END
END