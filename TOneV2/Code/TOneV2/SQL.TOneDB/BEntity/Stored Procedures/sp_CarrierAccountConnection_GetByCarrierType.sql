-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_CarrierAccountConnection_GetByCarrierType]

@CarrierType INT

AS
BEGIN
	SELECT cac.ID,
		   s.Name AS SwitchName,
		   cac.ConnectionType,
		   cac.TAG,cac.Value,
		   cac.GateWay 
	FROM   [CarrierAccountConnection] cac 
	JOIN   CarrierAccount ca ON cac.CarrierAccountID=ca.CarrierAccountID 
	JOIN   Switch s ON cac.SwitchID=s.SwitchID  
	WHERE  ca.ActivationStatus!=0 AND ca.AccountType IN (1,@CarrierType)
END