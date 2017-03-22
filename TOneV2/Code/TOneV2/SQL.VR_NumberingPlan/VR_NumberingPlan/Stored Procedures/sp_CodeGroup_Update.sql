-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_NumberingPlan].[sp_CodeGroup_Update]
	@ID int,	
	@CountryID int,
	@Code varchar(20)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM VR_NumberingPlan.[CodeGroup] WHERE ID != @ID AND Code = @Code)
	BEGIN
		Update VR_NumberingPlan.CodeGroup
	Set Code = @Code ,
		CountryID = @CountryID
	Where ID = @ID
	END
END