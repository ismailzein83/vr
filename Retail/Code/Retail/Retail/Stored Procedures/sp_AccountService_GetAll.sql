-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [Retail].[sp_AccountService_GetAll]
AS
BEGIN
	SELECT ID, AccountId, ServiceTypeID,ServiceChargingPolicyID, Settings
	FROM Retail.AccountService
END