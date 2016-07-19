

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [Mediation_Generic].[sp_MediationDefinition_Update]
	@ID INT,
	@Name nvarchar(255),
	@Details VARCHAR(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM [Mediation_Generic].[MediationDefinition] WHERE ID != @ID AND Name = @Name)
	BEGIN
		Update [Mediation_Generic].[MediationDefinition]
		Set Name = @Name, Details = @Details 
		Where ID = @ID
	END
END