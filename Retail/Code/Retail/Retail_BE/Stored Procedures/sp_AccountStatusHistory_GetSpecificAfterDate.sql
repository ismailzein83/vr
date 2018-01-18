-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail_BE].[sp_AccountStatusHistory_GetSpecificAfterDate]
	@AccountBEDefinitionID uniqueidentifier,
	@AccountIds nvarchar(MAX),
	@StatusChangedDate datetime
AS
BEGIN
	BEGIN
		DECLARE @AccountIdsTable TABLE (AccountId varchar(50))
		INSERT INTO @AccountIdsTable (AccountId)
		select ParsedString from [Retail_BE].[ParseStringList](@AccountIds)

		SELECT ID, AccountBEDefinitionID, AccountID, StatusId, PreviousStatusID, StatusChangedDate 
		FROM [Retail_BE].AccountStatusHistory 
		WHERE 
		AccountBEDefinitionID = @AccountBEDefinitionID 
		AND AccountID IN (SELECT AccountId FROM @AccountIdsTable)
		AND StatusChangedDate > @StatusChangedDate
		AND ISNULL(IsDeleted,0) = 0
	END
END