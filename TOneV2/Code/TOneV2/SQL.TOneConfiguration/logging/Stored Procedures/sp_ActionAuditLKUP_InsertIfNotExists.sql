
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [logging].[sp_ActionAuditLKUP_InsertIfNotExists]
	@Type int,
	@Name varchar(255)
AS
BEGIN
	INSERT INTO [logging].[ActionAuditLKUP] 
	([Type], Name)
	SELECT @Type, @Name
	WHERE NOT EXISTS (SELECT NULL FROM [logging].[ActionAuditLKUP] WITH (NOLOCK) WHERE [Type] = @Type AND Name = @Name)

    SELECT ID FROM [logging].[ActionAuditLKUP] WITH (NOLOCK) WHERE [Type] = @Type AND Name = @Name
END