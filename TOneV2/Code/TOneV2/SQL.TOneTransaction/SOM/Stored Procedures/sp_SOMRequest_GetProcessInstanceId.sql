-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE SOM.sp_SOMRequest_GetProcessInstanceId 
	@ID uniqueidentifier
AS
BEGIN
	SELECT ProcessInstanceID FROM SOM.SOMRequest WITH (NOLOCK)
	WHERE ID = @ID
END