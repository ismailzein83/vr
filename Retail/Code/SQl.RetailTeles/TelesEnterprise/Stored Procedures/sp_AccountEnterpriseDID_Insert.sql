CREATE PROCEDURE [TelesEnterprise].[sp_AccountEnterpriseDID_Insert]
	@AccountEnterpriseDIDTable [TelesEnterprise].[AccountEnterpriseDID] READONLY
AS
BEGIN
	DELETE [TelesEnterprise].[AccountEnterpriseDID]
	
	Insert into [TelesEnterprise].[AccountEnterpriseDID] (AccountId,EnterpriseId,ScreenNumber,[Type],[MaxCalls])
	Select AccountId,EnterpriseId,ScreenNumber,[Type],[MaxCalls] FROM @AccountEnterpriseDIDTable
END