Create PROCEDURE [QM_CLITester].[sp_TestCall_Update]
	@ID int,
	@SupplierID int,
	@CountryID int,
	@ZoneID int,
	@Test_ID nvarchar(100),
	@Name nvarchar(100),
	@Calls_Total int,
	@Calls_Complete int,
	@CLI_Success int,
	@CLI_No_Result int,
	@CLI_Fail int,
	@PDD int,
	@Status int
	
AS
BEGIN

SELECT 1 FROM QM_CLITester.TestCall WHERE ID = @Id
	BEGIN
		Update QM_CLITester.TestCall
		Set 
			SupplierID = @SupplierID,
			CountryID =	@CountryID,
			ZoneID = @ZoneID,
			Test_ID = @Test_ID,
			Name = @Name,
			Calls_Total = @Calls_Total,
			Calls_Complete = @Calls_Complete,
			CLI_Success = @CLI_Success,
			CLI_No_Result= @CLI_No_Result,
			CLI_Fail = @CLI_Fail,
			PDD  = @PDD,
			[Status] = @Status
			
	Where ID = @ID
	END



	
END