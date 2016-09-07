


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [QM_BE].[sp_Zone_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [ID]
      ,[Name]
      ,[SourceZoneID]
      ,[CountryID]
	  ,[BED]
	  ,[EED]
	  ,[Settings]
	  ,[IsFromTestingConnectorZone]
      from QM_BE.Zone WITH(NOLOCK) 
END