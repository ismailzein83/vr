-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [BEntity].[sp_CarrierGroup_GetCarrierGroup]
	 (@CarrierGroupId INT =  NULL)
AS
BEGIN
	SET NOCOUNT ON;

SELECT *
FROM CarrierGroup cg
			WHERE 
				cg.CarrierGroupID = @CarrierGroupId

END