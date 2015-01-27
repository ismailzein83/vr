-- ========================================================
-- Author:		Hassan Kheir Eddine
-- Create date: 2009-03-02
-- Description:	Get a table of Exchange Rates based on Date
-- =========================================================
CREATE FUNCTION [dbo].[GetZoneRateOfForcedSpecialRequest]
(
)
RETURNS 
@SpecialRequestRates TABLE 
(
	ZoneID int,
    Code varchar(20),
	SupplierID varchar(5),
	NormalRate real
	PRIMARY KEY(ZoneID, Code)
)
AS
BEGIN

INSERT INTO @SpecialRequestRates
(
	ZoneID,
	Code,
	SupplierID,
	NormalRate
)
SELECT 
    sr.ZoneID,
    sr.Code,
    sr.SupplierID,
    zr.NormalRate 
FROM  
     SpecialRequest sr WITH(NOLOCK,INDEX(IX_SpecialRequest_Supplier,IX_SpecialRequest)) ,
     CodeMatch cm  WITH(NOLOCK,INDEX(IDX_CodeMatch_Code,IDX_CodeMatch_Supplier,IDX_CodeMatch_Zone)) ,
     ZoneRate zr  WITH(NOLOCK,INDEX(IX_ZoneRate_Supplier,IX_ZoneRate_Zone))
WHERE sr.SpecialRequestType = 1
AND sr.IsEffective = 'Y' 
AND sr.SupplierID = zr.SupplierID 
AND cm.SupplierID = sr.SupplierID
AND (sr.ZoneID = zr.ZoneID OR (sr.Code = cm.Code AND cm.SupplierZoneID = zr.ZoneID))
	
RETURN
END