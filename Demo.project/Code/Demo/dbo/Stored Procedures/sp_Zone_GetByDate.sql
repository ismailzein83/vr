
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_Zone_GetByDate]
	-- Add the parameters for the stored procedure here
	@CountryId INT,
	@When DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT sz.[ID]
		  ,sz.[Name]
		  ,sz.CountryID
		  ,sz.BED
		  ,sz.EED
	  FROM [dbo].Zone sz
	  Where (sz.EED is null or sz.EED > @when) and sz.CountryId = @CountryId
	  
END