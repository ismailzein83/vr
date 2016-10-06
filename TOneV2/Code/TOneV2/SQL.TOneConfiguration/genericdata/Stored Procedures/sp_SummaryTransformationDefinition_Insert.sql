
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_SummaryTransformationDefinition_Insert] 
		@ID uniqueidentifier ,
	@Name nvarchar(255),
	@Details VARCHAR(MAX)

AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM genericdata.[SummaryTransformationDefinition] WHERE Name = @Name)
	BEGIN
		INSERT INTO genericdata.[SummaryTransformationDefinition](ID,Name,Details,CreatedTime)
		VALUES (@ID,@Name,@Details,GETDATE())

	END
END