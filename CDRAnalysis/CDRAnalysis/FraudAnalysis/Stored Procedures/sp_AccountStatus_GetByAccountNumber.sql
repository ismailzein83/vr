


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountStatus_GetByAccountNumber]
	@AccountNumber varchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT 
		AccountNumber, [Status],	ValidTill, Source, Reason, UserId,LastUpdatedOn
		
	FROM FraudAnalysis.AccountStatus WITH (NOLOCK)
	
	WHERE AccountNumber = @AccountNumber
	
END