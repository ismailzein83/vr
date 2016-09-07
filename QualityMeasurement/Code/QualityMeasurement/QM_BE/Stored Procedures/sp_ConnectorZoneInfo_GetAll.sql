
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [QM_BE].[sp_ConnectorZoneInfo_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT  [ID]
	  ,[ConnectorType]
      ,[ConnectorZoneID]
      ,[Codes]
      from QM_BE.ConnectorZoneInfo WITH(NOLOCK) 
END