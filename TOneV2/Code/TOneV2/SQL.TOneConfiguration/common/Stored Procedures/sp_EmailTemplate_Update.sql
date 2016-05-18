CREATE PROCEDURE [common].[sp_EmailTemplate_Update]
	@ID int,
	@Name nvarchar(255),
	@SubjectTemplate nvarchar(max),
	@BodyTemplate nvarchar(max)
AS
BEGIN
IF EXISTS(SELECT 1 FROM common.EmailTemplate WHERE ID = @ID )
	BEGIN
		Update common.EmailTemplate
		Set [Name] = @Name,
		[SubjectTemplate] = @SubjectTemplate,
		[BodyTemplate] = @BodyTemplate
		Where ID = @ID
	END
END