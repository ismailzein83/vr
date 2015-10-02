-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSource_GetAll] 

AS
BEGIN
	SET NOCOUNT ON;

	SELECT DS.[ID], DS.[Name]
    FROM integration.DataSource AS DS
    
    SET NOCOUNT OFF;
END