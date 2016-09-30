-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_BusinessEntityDefinition_Insert]
	-- Add the parameters for the stored procedure here
	@ID uniqueidentifier,
	@Name nvarchar(50),
	@Title nvarchar(50),
	@Settings nvarchar(max)

AS
BEGIN
IF NOT EXISTS(select 1 from genericdata.BusinessEntityDefinition where Name = @Name) 
	BEGIN	
		INSERT INTO  genericdata.BusinessEntityDefinition(ID,Name,Title,Settings) 
		VALUES (@ID,@Name,@Title,@Settings)

	END
END