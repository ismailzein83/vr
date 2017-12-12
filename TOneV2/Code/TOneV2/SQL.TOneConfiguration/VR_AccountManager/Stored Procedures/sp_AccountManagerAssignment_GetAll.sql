-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE  [VR_AccountManager].[sp_AccountManagerAssignment_GetAll] 
@AssignmentDefinitionID uniqueidentifier
AS
BEGIN
	SELECT	ID,AssignmentDefinitionID,AccountManagerID,AccountID,Settings,BED,EED
	FROM	[VR_AccountManager].[AccountManagerAssignment] WITH(NOLOCK)
	WHERE	AssignmentDefinitionID=@AssignmentDefinitionID

    
END