-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_ChargingPolicy_GetAll]
AS
BEGIN
	SELECT ID, Name, ServiceTypeId, Settings
	FROM Retail.ChargingPolicy  with(nolock)
END