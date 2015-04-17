-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPInstance_ClearLoadedFlag]
AS
BEGIN
	UPDATE bp.BPInstance
	SET LoadedByRuntime = 0
END