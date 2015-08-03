-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [BEntity].[sp_CarrierGroup_GetAll] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [CarrierGroupID]
      ,[CarrierGroupName]
      ,[ParentID]
      ,[ParentPath]
      ,[Path] from [dbo].[CarrierGroup]
END