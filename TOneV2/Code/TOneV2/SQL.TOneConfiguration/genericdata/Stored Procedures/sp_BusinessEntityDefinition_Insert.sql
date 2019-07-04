-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_BusinessEntityDefinition_Insert]
	-- Add the parameters for the stored procedure here
	@ID uniqueidentifier,
	@Name nvarchar(900),
	@Title nvarchar(1000),
	@DevProjectId uniqueidentifier,
	@Settings nvarchar(max)

AS
BEGIN
IF NOT EXISTS(select 1 from genericdata.BusinessEntityDefinition where Name = @Name) 
	BEGIN	
		INSERT INTO  genericdata.BusinessEntityDefinition(ID,Name,Title,DevProjectId,Settings) 
		VALUES (@ID,@Name,@Title,@DevProjectId,@Settings)

	END
END