-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_AccountManager_GetAssignedCarriers] 
	@UserIds [BEntity].[MemberIdType] READONLY,
	@CarrierType SMALLINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT am.UserId, cp.Name AS CarrierName, ca.NameSuffix, am.CarrierAccountId, RelationType
	FROM BEntity.AccountManager am
	INNER JOIN dbo.CarrierAccount ca ON am.CarrierAccountId = ca.CarrierAccountID
	INNER JOIN dbo.CarrierProfile cp ON ca.ProfileID = cp.ProfileID
	WHERE am.UserId IN (SELECT Id FROM @UserIds) AND (@CarrierType = 1 OR am.RelationType = @CarrierType)
END