-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_AccountService_Update]
	@ID bigint,
	@AccountId bigint,
	@ServiceTypeID uniqueidentifier,
	@ServiceChargingPolicyID INT,
	@Settings NVARCHAR(MAX)

AS
BEGIN
	UPDATE Retail.AccountService
	SET AccountId = @AccountId, ServiceTypeID = @ServiceTypeID,ServiceChargingPolicyID=@ServiceChargingPolicyID ,Settings = @Settings
	WHERE ID = @ID
END