
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_SummaryTransformationDefinition_Insert] 
	@Name nvarchar(255),
	@Details VARCHAR(MAX),
	@ID INT OUT
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM genericdata.[SummaryTransformationDefinition] WHERE Name = @Name)
	BEGIN
		INSERT INTO genericdata.[SummaryTransformationDefinition](Name,Details,CreatedTime)
		VALUES (@Name,@Details,GETDATE())
		SET @ID = SCOPE_IDENTITY() 
	END
END