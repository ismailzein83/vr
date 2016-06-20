-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [Retail].[sp_AccountService_Insert]
	@AccountId bigint,
	@ServiceTypeID INT,
	@ServiceChargingPolicyID INT,
	@Settings NVARCHAR(MAX),
	@ID bigint OUT
AS
BEGIN
	INSERT INTO Retail.AccountService(AccountId, ServiceTypeID, ServiceChargingPolicyID, Settings)
	VALUES (@AccountId, @ServiceTypeID, @ServiceChargingPolicyID, @Settings)
	SET @ID = @@IDENTITY
END