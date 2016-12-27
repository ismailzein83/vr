-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [NP_IVSwitch].[sp_TranslationRule_Update] 
	-- Add the parameters for the stored procedure here
	@ID int,
	@Name nvarchar(255) , 
	@DNIS_Pattern varchar(255),
	@CLI_Pattern varchar(255) 
AS
BEGIN
	-- Insert statements for procedure here
	IF NOT EXISTS(SELECT 1 FROM NP_IVSwitch.TranslationRule WHERE ID != @ID and Name = @Name)
	BEGIN
	UPDATE NP_IVSwitch.TranslationRule
	SET Name=@Name, DNIS_Pattern = @DNIS_Pattern, CLI_Pattern = @CLI_Pattern
	WHERE ID = @ID
	END
END