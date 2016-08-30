-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_DataTransformationDefinition_Insert]
	@Name nvarchar(255),
	@Title nvarchar(255),
	@Details VARCHAR(MAX),
	@ID INT OUT
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM genericdata.DataTransformationDefinition WHERE Name = @Name)
	BEGIN
		INSERT INTO genericdata.DataTransformationDefinition(Name,Title,Details)
		VALUES (@Name,@Title,@Details)
		SET @ID = SCOPE_IDENTITY() 
	END
END