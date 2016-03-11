-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [InterConnect_BE].[sp_OperatorAccount_Update]
    @ID INT,
    @Suffix nvarchar(255),
    @ProfileId int
	
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM InterConnect_BE.OperatorAccount WHERE ID != @ID AND Suffix =@Suffix and @ProfileId = ProfileID)
	BEGIN
		UPDATE InterConnect_BE.OperatorAccount
		SET Suffix=@Suffix, ProfileID = @ProfileId 
		WHERE ID = @ID
	END
END