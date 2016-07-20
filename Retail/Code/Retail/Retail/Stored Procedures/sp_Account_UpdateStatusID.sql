-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE Retail.sp_Account_UpdateStatusID
	@ID BIGINT,
	@StatusID uniqueidentifier
AS
BEGIN
		UPDATE Retail.Account
		SET StatusID = @StatusID
		WHERE ID = @ID
END