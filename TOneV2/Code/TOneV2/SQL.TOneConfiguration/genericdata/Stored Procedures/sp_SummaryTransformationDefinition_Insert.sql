
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [genericdata].[sp_SummaryTransformationDefinition_Insert] 
	@Name nvarchar(255),
	@Details VARCHAR(MAX),
	@ID INT OUT
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM genericdata.[SummaryTransformationDefinition] WHERE Name = @Name)
	BEGIN
		INSERT INTO genericdata.[SummaryTransformationDefinition](Name,Details,CreatedTime)
		VALUES (@Name,@Details,GETDATE())
		SET @ID = @@IDENTITY 
	END
END