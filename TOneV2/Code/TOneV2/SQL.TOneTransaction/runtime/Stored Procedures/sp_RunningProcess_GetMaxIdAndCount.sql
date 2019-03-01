-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_RunningProcess_GetMaxIdAndCount]
AS
BEGIN
	SELECT MAX(ID) MaxID, COUNT(*) NbOfProcesses FROM runtime.RunningProcess WITH(NOLOCK)
	WHERE ISNULL(IsDraft, 0) = 0
END