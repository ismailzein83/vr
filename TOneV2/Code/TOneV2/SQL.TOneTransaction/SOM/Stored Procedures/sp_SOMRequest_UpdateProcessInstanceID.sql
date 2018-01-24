-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [SOM].[sp_SOMRequest_UpdateProcessInstanceID] 
	@ID uniqueidentifier,
	@ProcessInstanceID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	UPDATE [SOM].[SOMRequest]
    SET ProcessInstanceID = @ProcessInstanceID
	WHERE ID = @ID
END