-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_HoldRequest_GetAll]
AS
BEGIN
	Select ID, [BPInstanceID],[ExecutionFlowDefinitionId], [From], [To], [QueuesToHold], [QueuesToProcess], [Status], Createdtime
	From [queue].[HoldRequest] WITH(NOLOCK)
	order by ID
END