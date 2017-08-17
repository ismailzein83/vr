-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail_BE].[sp_AccountStatusHistory_Insert]
	@AccountBEDefinitionID uniqueidentifier,
	@AccountID BIGINT,
	@StatusId uniqueidentifier
AS
BEGIN
	
	BEGIN
		INSERT INTO [Retail_BE].AccountStatusHistory ([AccountBEDefinitionID], AccountID, StatusId, StatusChangedDate)
		VALUES (@AccountBEDefinitionID, @AccountID, @StatusId,GETDATE())
	END
END