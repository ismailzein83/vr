﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_AccountBalance].[sp_LiveBalance_GetById]
	-- Add the parameters for the stored procedure here
	@AccountID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
/****** Script for SelectTopNRows command from SSMS  ******/
	SELECT  AccountID,
			CurrencyID,
			InitialBalance,
			UsageBalance,
			CurrentBalance,
			CurrentAlertThreshold,
			NextAlertThreshold,
			AlertRuleID
	FROM	[VR_AccountBalance].LiveBalance lb
	WHERE AccountID = @AccountID
        
END