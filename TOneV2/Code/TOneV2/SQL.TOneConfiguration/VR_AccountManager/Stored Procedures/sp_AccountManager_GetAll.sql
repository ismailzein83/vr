-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountManager].[sp_AccountManager_GetAll] 

AS
BEGIN
	SELECT	ID,AccountManagerDefinitionID,UserID,Settings
	FROM	[VR_AccountManager].[AccountManager] WITH(NOLOCK)
END