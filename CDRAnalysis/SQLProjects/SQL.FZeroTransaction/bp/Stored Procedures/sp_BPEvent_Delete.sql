-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPEvent_Delete]
	@ID bigint
	
AS
BEGIN
	DELETE FROM [bp].[BPEvent]
      WHERE ID = @ID
END