-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_GetById]
	-- Add the parameters for the stored procedure here
	@AccountTypeId uniqueidentifier,
	@AccountID varchar(50)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
/****** Script for SelectTopNRows command from SSMS  ******/
	SELECT   AccountID
			,AccountTypeID
			,CurrencyID
			,InitialBalance
			,CurrentBalance
			,NextAlertThreshold
			,AlertRuleID
			,LastExecutedActionThreshold
			,ActiveAlertsInfo
			,BED
			,EED
			,[Status]
	FROM	[VR_AccountBalance].LiveBalance lb  with(nolock)
	WHERE	AccountTypeID = @AccountTypeId and AccountID = @AccountID AND ISNULL(IsDeleted,0) = 0
        
END