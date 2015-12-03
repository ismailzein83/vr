-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [QM_CLITester].[sp_TestCall_Insert]
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
	@Status int,
	@Id int out
AS
BEGIN

	Insert into QM_CLITester.TestCall([SupplierID], [CountryID], [ZoneID], [CreationDate], [Test_ID],[Name],[Calls_Total],[Calls_Complete],[CLI_Success] ,[CLI_No_Result], [CLI_Fail], [PDD], [Status])
	Values(@SupplierID, @CountryID, @ZoneID, GETDATE(), @Test_ID, @Name,@Calls_Total, @Calls_Complete, @CLI_Success,@CLI_No_Result, @CLI_No_Result, @PDD, @Status)
	
	Set @Id = @@IDENTITY
END