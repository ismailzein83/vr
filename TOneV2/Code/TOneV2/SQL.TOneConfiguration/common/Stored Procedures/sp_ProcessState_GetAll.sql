
CREATE PROCEDURE [common].[sp_ProcessState_GetAll]
AS
BEGIN
	Select UniqueName, Settings 
	from common.ProcessState WITH(NOLOCK)
END