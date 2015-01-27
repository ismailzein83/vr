CREATE VIEW vw_RouteBlocks
AS
SELECT TOP 100 percent * FROM RouteBlock rb 
WHERE rb.IsEffective = 'Y'
ORDER BY rb.CustomerID, rb.Code