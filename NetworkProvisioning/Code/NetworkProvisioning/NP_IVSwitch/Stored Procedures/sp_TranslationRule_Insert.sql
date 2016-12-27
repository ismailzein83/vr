-- =============================================
-- Author:		Name
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [NP_IVSwitch].[sp_TranslationRule_Insert] 
	-- Add the parameters for the stored procedure here
	@Name nvarchar(255) , 
	@DNIS_Pattern varchar(255),
	@CLI_Pattern varchar(255),
	@ID int out
AS
BEGIN
	

    -- Insert statements for procedure here
	IF NOT EXISTS(SELECT 1 FROM NP_IVSwitch.TranslationRule WHERE  Name = @Name)
	BEGIN
	INSERT INTO NP_IVSwitch.TranslationRule(Name, DNIS_Pattern,CLI_Pattern) values(@Name, @DNIS_Pattern,@CLI_Pattern)
	END
	SET @ID = SCOPE_IDENTITY()
END