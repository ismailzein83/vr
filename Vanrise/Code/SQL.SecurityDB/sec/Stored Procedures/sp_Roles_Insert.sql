-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_Roles_Insert] 
	@Name Nvarchar(255),
	@Description ntext,
	@Id int out
AS
BEGIN
IF NOT EXISTS(select 1 from sec.[Role] where Name = @Name)
	BEGIN
		Insert into [sec].[Role] ([Name], [Description])
		values(@Name, @Description)
		
		SET @Id = @@IDENTITY
	END
END