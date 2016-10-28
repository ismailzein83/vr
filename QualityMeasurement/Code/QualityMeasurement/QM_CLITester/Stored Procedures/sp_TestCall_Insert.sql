-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [QM_CLITester].[sp_TestCall_Insert]
	@SupplierID int,
	@CountryID int,
	@ZoneID int,
	@CallTestStatus int,
	@CallTestResult int,
	@InitiationRetryCount int,
	@GetProgressRetryCount int,
	@UserID int,
	@ProfileID int,
	@BatchNumber bigint,
	@scheduleID uniqueidentifier,
	@Quantity int,
	@ID int out
AS
BEGIN

	Insert into QM_CLITester.TestCall([SupplierID], [CountryID], [ZoneID], [CreationDate],
	[CallTestStatus],[CallTestResult],[UserID], [ProfileID], [InitiationRetryCount], [GetProgressRetryCount], [BatchNumber], [scheduleID], [Quantity], [UpdateStatusTime])
	Values(@SupplierID, @CountryID, @ZoneID, GETDATE(),@CallTestStatus, @CallTestResult, @UserID, @ProfileID, @InitiationRetryCount, @GetProgressRetryCount, @BatchNumber, @scheduleID, @Quantity, GETDATE())
	
	Set @ID = SCOPE_IDENTITY()
END