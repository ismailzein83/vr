-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [Retail].[sp_AccountService_UpdateStatusID]
	@ID BIGINT,
	@StatusID uniqueidentifier
AS
BEGIN
		UPDATE Retail.AccountService
		SET StatusID = @StatusID
		WHERE ID = @ID
END