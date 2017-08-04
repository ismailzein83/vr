

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountStatus_BulkUpdate]
	@AccountStatus [FraudAnalysis].AccountStatusType READONLY,
	@ValidTill datetime, 
	@Source int,
	@Reason varchar(max),
	@UserId int
AS
BEGIN
	
	UPDATE [FraudAnalysis].[AccountStatus] 
	SET   AccountStatus.ValidTill = @ValidTill, AccountStatus.[Status]=4, Source=@Source, Reason=@Reason, UserId = @UserId,LastUpdatedOn = getdate()
	FROM [FraudAnalysis].[AccountStatus] WITH(NOLOCK) inner join @AccountStatus as ac ON  AccountStatus.AccountNumber = ac.AccountNumber
	

	INSERT INTO [FraudAnalysis].[AccountStatus]
    ([AccountNumber]   ,[Status]  ,[ValidTill], [Source], [Reason], [UserId],LastUpdatedOn)
	SELECT  ac.[AccountNumber] , 4, @ValidTill , @Source, @Reason, @UserId,getdate()
	FROM    @AccountStatus as ac 
	LEFT JOIN
			[FraudAnalysis].[AccountStatus] WITH(NOLOCK)
	ON      AccountStatus.AccountNumber = ac.AccountNumber
	WHERE   AccountStatus.AccountNumber IS NULL
	
END