-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [genericdata].[sp_DataTransformationDefinition_Update]
	@ID INT,
	@Name nvarchar(255),
	@Title nvarchar(255),
	@Details VARCHAR(MAX)
	
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM genericdata.DataTransformationDefinition WHERE ID != @ID AND Name = @Name)
	BEGIN
		Update genericdata.DataTransformationDefinition
		Set Name = @Name, Title = @Title , Details = @Details
		Where ID = @ID
	END
END