
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_SummaryTransformationDefinition_Update]
	@ID uniqueidentifier,
	@Name nvarchar(255),
	@DevProjectId uniqueidentifier,
	@Details VARCHAR(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM genericdata.[SummaryTransformationDefinition] WHERE ID != @ID AND Name = @Name)
	BEGIN
		Update genericdata.[SummaryTransformationDefinition]
		Set Name = @Name,DevProjectID=@DevProjectId, Details = @Details, LastModifiedTime = GETDATE()
		Where ID = @ID
	END
END