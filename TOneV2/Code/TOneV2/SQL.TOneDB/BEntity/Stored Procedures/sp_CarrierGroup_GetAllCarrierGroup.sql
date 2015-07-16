-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [BEntity].[sp_CarrierGroup_GetAllCarrierGroup]
AS
BEGIN
	SET NOCOUNT ON;

SELECT  cg.CarrierGroupID, 
		cg.CarrierGroupName,
		cg.ParentID,
		cg.ParentPath,
		cg.[Path]
FROM CarrierGroup cg

END