
CREATE FUNCTION [dbo].[GetRepresentedAsSwitchCarriers]()
RETURNS
@RepresentedAsSwitch TABLE
(
CID VARCHAR(5)	
)
AS 
BEGIN 
	 
INSERT INTO @RepresentedAsSwitch   SELECT ca.CarrierAccountID  
                                   FROM CarrierAccount ca WITH (NOLOCK) WHERE ca.RepresentsASwitch='Y'
                                   

RETURN

END