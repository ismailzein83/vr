-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_CarrierAccount_UpdateGroups] 
(
@CarrierAccountId varchar(4),
@CarrierGroupID int,
@CarrierGroups varchar(50)
)
AS	
BEGIN
UPDATE CarrierAccount
SET CarrierGroupID = @CarrierGroupID,
	CarrierGroups = @CarrierGroups where CarrierAccountId = @CarrierAccountId
END