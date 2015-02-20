CREATE PROCEDURE [BEntity].[sp_CarrierAccount_GetActiveSuppliersInfo]
AS
BEGIN
	SELECT CarrierAccountID
    FROM CarrierAccount ca  WITH(NOLOCK)
    WHERE    CarrierAccountID = 'SYS'
     OR(
				ca.IsDeleted = 'N' 
			AND ca.ActivationStatus IN (2, 1) --(byte)ActivationStatus.Active, (byte)ActivationStatus.Testing
			AND ca.RoutingStatus IN (3, 1) --(byte)RoutingStatus.Enabled, (byte)RoutingStatus.BlockedInbound
        )
	
END