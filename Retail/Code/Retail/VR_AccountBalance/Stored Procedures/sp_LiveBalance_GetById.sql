-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_GetById]
	-- Add the parameters for the stored procedure here
	@AccountTypeId uniqueidentifier,
	@AccountID bigint
	
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
	FROM	[VR_AccountBalance].LiveBalance lb  with(nolock)
	WHERE	AccountTypeID = @AccountTypeId and AccountID = @AccountID
        
END