-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_BusinessEntityDefinition_Insert]
	-- Add the parameters for the stored procedure here
	@Name nvarchar(50),
	@Title nvarchar(50),
	@Settings nvarchar(max),
	@ID int out
AS
BEGIN
IF NOT EXISTS(select 1 from genericdata.BusinessEntityDefinition where Name = @Name) 
	BEGIN	
		INSERT INTO  genericdata.BusinessEntityDefinition(Name,Title,Settings) 
		VALUES (@Name,@Title,@Settings)
		SET @Id = SCOPE_IDENTITY()
	END
END