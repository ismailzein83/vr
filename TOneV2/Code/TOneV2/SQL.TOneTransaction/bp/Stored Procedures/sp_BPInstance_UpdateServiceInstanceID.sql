-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPInstance_UpdateServiceInstanceID]	
	@ID bigint,
	@ServiceInstanceID uniqueidentifier
	
AS
BEGIN	
	
    UPDATE bp.BPInstance
    SET	ServiceInstanceID = @ServiceInstanceID
	WHERE ID = @ID
END