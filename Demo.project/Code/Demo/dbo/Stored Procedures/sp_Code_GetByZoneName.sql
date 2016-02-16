
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [dbo].[sp_Code_GetByZoneName]
	-- Add the parameters for the stored procedure here
	@ZoneName nvarchar(255),
	@When DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
/****** Script for SelectTopNRows command from SSMS  ******/
	SELECT  sc.[ID],
			sc.[Code],
			sc.[ZoneID],
			sc.[BED],
			sc.[EED]
	FROM	[dbo].[Code] sc
	join [dbo].[Zone] sz on sz.Id = sc.[ZoneID]
	WHERE  sz.Name = @ZoneName and ((sc.BED <= @when ) and (sc.EED is null or sc.EED > @when))
        
END