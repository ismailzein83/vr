-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail_BE].sp_AccountStatusHistory_Insert
	@AccountID BIGINT,
	@StatusId uniqueidentifier,
	@StatusChangedDate DATETIME,
	@ID BIGINT OUT
AS
BEGIN
	
	BEGIN
		INSERT INTO [Retail_BE].AccountStatusHistory (AccountID, StatusId, StatusChangedDate)
		VALUES (@AccountID, @StatusId,@StatusChangedDate)
		SET @ID = SCOPE_IDENTITY()
	END
END