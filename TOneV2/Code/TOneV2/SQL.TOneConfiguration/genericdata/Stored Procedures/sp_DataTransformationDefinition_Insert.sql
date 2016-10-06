-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_DataTransformationDefinition_Insert]
		@ID uniqueidentifier ,
	@Name nvarchar(255),
	@Title nvarchar(255),
	@Details VARCHAR(MAX)

AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM genericdata.DataTransformationDefinition WHERE Name = @Name)
	BEGIN
		INSERT INTO genericdata.DataTransformationDefinition(ID,Name,Title,Details)
		VALUES (@ID,@Name,@Title,@Details)

	END
END