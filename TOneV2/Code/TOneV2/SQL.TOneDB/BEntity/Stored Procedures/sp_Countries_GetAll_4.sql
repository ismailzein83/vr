-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE BEntity.sp_Countries_GetAll
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT lk.LookupValueID AS ID, 
			lk.Value AS [Description]
	FROM [dbo].[LookupValue] lk JOIN [dbo].[LookupType] lkType ON  lk.LookupTypeID = lkType.LookupTypeID
	WHERE lkType.Name= 'Countries'

END