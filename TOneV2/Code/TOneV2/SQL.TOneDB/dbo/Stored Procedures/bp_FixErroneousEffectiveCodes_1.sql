CREATE PROCEDURE [dbo].[bp_FixErroneousEffectiveCodes]
AS
UPDATE Code SET EndEffectiveDate = 
	(
		SELECT TOP 1 CN.BeginEffectiveDate FROM Code CN, Zone ZN, Zone ZO 
			WHERE 
				ZO.ZoneID = Code.ZoneID
			AND ZO.SupplierID = ZN.SupplierID 
			AND ZN.ZoneID = CN.ZoneID
			AND CN.Code = Code.Code 
			AND (
				(CN.BeginEffectiveDate > Code.BeginEffectiveDate AND CN.ID <> Code.ID) 
				OR 
				(CN.BeginEffectiveDate = Code.BeginEffectiveDate AND CN.ID > Code.ID)
				) 				
			AND CN.EndEffectiveDate IS NULL
			ORDER BY CN.BeginEffectiveDate ASC
	)
WHERE 
		Code.EndEffectiveDate IS NULL
	AND EXISTS 
		(
			SELECT * FROM Code CN, Zone ZN, Zone ZO 
				WHERE 
					ZO.ZoneID = Code.ZoneID
				AND ZO.SupplierID = ZN.SupplierID 
				AND ZN.ZoneID = CN.ZoneID
				AND CN.Code = Code.Code 
				AND (
					(CN.BeginEffectiveDate > Code.BeginEffectiveDate AND CN.ID <> Code.ID) 
					OR 
					(CN.BeginEffectiveDate = Code.BeginEffectiveDate AND CN.ID > Code.ID)
					) 				
				AND CN.EndEffectiveDate IS NULL
		)