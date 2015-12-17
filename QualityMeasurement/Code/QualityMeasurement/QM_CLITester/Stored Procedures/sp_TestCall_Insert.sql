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
	@ID int out
AS
BEGIN

	Insert into QM_CLITester.TestCall([SupplierID], [CountryID], [ZoneID], [CreationDate],
	[CallTestStatus],[CallTestResult],[UserID], [ProfileID], [InitiationRetryCount], [GetProgressRetryCount])
	Values(@SupplierID, @CountryID, @ZoneID, GETDATE(),@CallTestStatus, @CallTestResult, @UserID, @ProfileID, @InitiationRetryCount, @GetProgressRetryCount)
	
	Set @ID = @@IDENTITY
END