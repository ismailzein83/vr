
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_Code_GetByDate]
	-- Add the parameters for the stored procedure here
	@CountryId INT,
	@When DateTime
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT sc.[ID]
		  ,sc.Code
		  ,sc.[CodeGroupID]
		  ,sc.ZoneID
		  ,sc.BED
		  ,sc.EED
	  FROM [dbo].Code sc LEFT JOIN [dbo].Zone sz ON sc.ZoneID=sz.ID 
	  Where  (sc.EED is null or sc.EED > @when) and sz.CountryID = @CountryId 
END