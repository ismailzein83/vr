-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE VR_AccountBalance.sp_AlertRuleActionExecution_Delete
	@ID bigint 
AS
BEGIN
		DELETE FROM VR_AccountBalance.AlertRuleActionExecution
        WHERE ID = @Id 
END