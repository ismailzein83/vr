-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_AccountService_GetAll]
AS
BEGIN
	SELECT ID, AccountId, ServiceTypeID,ServiceChargingPolicyID, Settings,StatusID,ExecutedActionsData
	FROM Retail.AccountService  with(nolock)
END