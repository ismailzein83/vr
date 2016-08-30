-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_BusinessEntityModule_Insert]
	-- Add the parameters for the stored procedure here
	@Name nvarchar(255),
	@ParentId int,
	@BreakInheritance bit,
	@Id int out
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
IF NOT EXISTS(select 1 from sec.BusinessEntityModule where Name = @Name)
	BEGIN	
		INSERT INTO sec.[BusinessEntityModule](Name,ParentId,BreakInheritance) 
		VALUES (@Name,@ParentId,@BreakInheritance)
	 -- Insert statements for procedure here
		SET @Id = SCOPE_IDENTITY()
	END
END