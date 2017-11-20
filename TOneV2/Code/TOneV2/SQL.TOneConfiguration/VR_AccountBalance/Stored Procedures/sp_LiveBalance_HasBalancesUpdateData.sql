-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_HasBalancesUpdateData]
@AccountTypeId uniqueidentifier

AS
BEGIN
	IF EXISTS(Select TOP 1 ID from VR_AccountBalance.LiveBalance lb WITH(NOLOCK) 
	where AccountTypeID = @AccountTypeID 
	AND ((lb.[CurrentBalance] <= lb.NextAlertThreshold AND (lb.LastExecutedActionThreshold IS NULL OR lb.NextAlertThreshold  < lb.LastExecutedActionThreshold) AND ISNULL(IsDeleted,0) = 0) 
		 OR (lb.[CurrentBalance] > lb.LastExecutedActionThreshold AND ISNULL(IsDeleted,0) = 0)))
	BEGIN
		SELECT 1
	END

	ELSE
	BEGIN
		SELECT 0
	END
END