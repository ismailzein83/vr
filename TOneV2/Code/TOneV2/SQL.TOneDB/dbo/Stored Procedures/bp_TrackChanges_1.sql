CREATE PROCEDURE [dbo].[bp_TrackChanges](
 @ZoneID int ,
 @CodeID int=   NULL,
 @ChangesFlag smallint 
 )
AS 
BEGIN 
	
	
  TRUNCATE TABLE Changes 

INSERT  INTO Changes
(
	CustomerID,
	ZoneID,
	CodeID,
	Changes     
)
SELECT Distinct 
           P.CustomerID ,R.ZoneID,C.ID, @ChangesFlag
FROM PriceList P    
          JOIN Rate R ON R.PriceListID = P.PriceListID
          JOIN Code C ON C.ZoneID = R.ZoneID
WHERE R.IsEffective = 'Y' 
          AND P.SupplierID = 'SYS'
          AND (@CodeID IS NULL OR C.ID= @CodeID)
          AND R.ZoneID = @ZoneID
ORDER BY P.CustomerID
    
    
END