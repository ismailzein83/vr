-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_Account_UpdateStatusID]
	@ID BIGINT,
	@StatusID uniqueidentifier,
	@LastModifiedBy int
AS
BEGIN
		UPDATE Retail.Account
		SET StatusID = @StatusID, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
		WHERE ID = @ID
END