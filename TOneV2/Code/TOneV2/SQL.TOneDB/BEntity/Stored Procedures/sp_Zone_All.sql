-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Zone_All]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	SELECT z.ZoneID,z.CodeGroup as CodeGroupId,z.Name, z.SupplierID, z.ServicesFlag,z.BeginEffectiveDate,z.EndEffectiveDate,cg.Name CodeGroupName
	FROM Zone z
	JOIN CodeGroup cg 
	ON z.CodeGroup=cg.Code
END