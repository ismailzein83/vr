
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_SummaryTransformationDefinition_Insert] 
		@ID uniqueidentifier ,
	@Name nvarchar(255),
	@DevProjectId uniqueidentifier,
	@Details VARCHAR(MAX)

AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM genericdata.[SummaryTransformationDefinition] WHERE Name = @Name)
	BEGIN
		INSERT INTO genericdata.[SummaryTransformationDefinition](ID,Name,DevProjectID,Details,CreatedTime)
		VALUES (@ID,@Name,@DevProjectId,@Details,GETDATE())

	END
END