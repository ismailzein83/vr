-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [InterConnect_BE].[sp_OperatorAccount_Insert]
    @Suffix nvarchar(255),
    @ProfileId int,
	@ID int OUT
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM InterConnect_BE.OperatorAccount WHERE Suffix = @Suffix and @ProfileId = ProfileID)
	BEGIN
		INSERT INTO  InterConnect_BE.OperatorAccount (Suffix, ProfileID)
		VALUES(@Suffix, @ProfileId)
		SET @ID = @@IDENTITY
	END
END