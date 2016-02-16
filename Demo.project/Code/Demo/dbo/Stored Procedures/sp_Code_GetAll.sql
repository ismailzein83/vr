
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_Code_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT  
	   sc.[ID]
      ,sc.[Code]
      ,sc.[ZoneID]
      ,sc.[BED]
      ,sc.[EED]
      from dbo.Code sc
END