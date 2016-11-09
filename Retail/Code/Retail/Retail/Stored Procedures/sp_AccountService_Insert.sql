-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_AccountService_Insert]
	@AccountId bigint,
	@ServiceTypeID uniqueidentifier,
	@ServiceChargingPolicyID INT,
	@Settings NVARCHAR(MAX),
	@StatusID uniqueidentifier,
	@ID bigint OUT
AS
BEGIN
	INSERT INTO Retail.AccountService(AccountId, ServiceTypeID, ServiceChargingPolicyID, Settings,StatusID)
	VALUES (@AccountId, @ServiceTypeID, @ServiceChargingPolicyID, @Settings,@StatusID)
	SET @ID = SCOPE_IDENTITY()
END