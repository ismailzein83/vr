
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [QM_BE].[sp_ConnectorResultMapping_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT  [ID]
      ,[ConnectorType]
      ,[ResultID]
      ,[ResultName]
      ,[ConnectorResults]
      from QM_BE.ConnectorResultMapping WITH(NOLOCK) 
END