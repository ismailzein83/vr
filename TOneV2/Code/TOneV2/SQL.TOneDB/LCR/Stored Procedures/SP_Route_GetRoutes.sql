-- =============================================
-- Author:		<Rayyan Kazma>
-- Create date: <2015-08-17>
-- Description:	<Procedure that retrieve data of Route and RouteOption>
-- =============================================
CREATE PROCEDURE [LCR].[SP_Route_GetRoutes]
 @From INT = 1,
 @TO INT = 10,
 @TopValues INT = 10,
 @IsBlock CHAR(1) = NULL,
 @showBlocks CHAR(1) = NULL,
 @Zone VARCHAR(100) = NULL,
 @Code VARCHAR(15) = NULL,
 @SupplierID VARCHAR(5) = NULL,
 @CustomerID VARCHAR(5) = NULL

AS
BEGIN

DECLARE @ParmDefinition NVARCHAR(500)
SET @ParmDefinition = N''


DECLARE @SQL NVARCHAR(MAX)




                      SET @SQL =   '
                      DECLARE @RouteOptionTable table(RouteID int,SupplierID varchar(5),SupplierZoneID int, SupplierServicesFlag smallint,SupplierActiveRate real,[Priority] tinyint,NumberOfTries tinyint,
					  Percentage tinyint, [State] tinyint) 
                      DECLARE @RouteIDVar int 
                      DECLARE @Index int = 1  
                      DECLARE @Count int  = 0
									SELECT TOP '+CONVERT(VARCHAR(10),@TopValues)+'
                                    RT.RouteID,
                                    RT.CustomerID,
                                    RT.Code,	
                                    RT.OurZoneID ,
                                    RT.OurServicesFlag,
                                    RT.OurActiveRate,
                                    RT.State,                                    
                                    RT.Updated,
                                    RT.IsBlockAffected,
                                    RT.IsSpecialRequestAffected,
                                    RT.IsToDAffected,
                                    RT.IsOverrideAffected,
                                    RT.IsOptionBlock
                                INTO #FirstTempTbl
                                FROM   [Route] RT WITH(NOLOCK'
                                IF(@Zone IS NULL)
                                BEGIN
                                SET @SQL = @SQL +') '
                                END
                                ELSE
                                BEGIN
								SET @SQL = @SQL +  ',INDEX(IX_Route_Zone)) '
                                END
                                IF(@SupplierID IS NOT NULL)
                                BEGIN
                                SET @SQL = @SQL + ',RouteOption ro WITH(NOLOCK)
                                    ,Zone sz with(nolock) '
                                END
                                IF(@Zone IS NOT NULL)
                                BEGIN
                                SET @SQL = @SQL + ' ,Zone OZ with(nolock) '
                                END
							    SET @SQL = @SQL + ' WHERE 1=1 '
							    IF(@SupplierID IS NOT NULL or @IsBlock is not null)
							    BEGIN
							    SET @SQL = @SQL+' AND RT.RouteID = ro.RouteID
	                                AND ro.SupplierZoneID = sz.ZoneID
	                                and ro.SupplierID = @SupplierIDParameter
	                                AND sz.SupplierID = @SupplierIDParameter
	                                '
	                           
                                END
                                IF(@IsBlock = 'N')
                                BEGIN
                                SET @SQL = @SQL + ' AND ro.[State] = 1 '
                                END
                                IF(@Zone IS NOT NULL)
                                BEGIN
                                SET @SQL = @SQL+' AND OZ.ZoneID = RT.OurZoneID '
                                END
                                IF(@CustomerID IS NOT NULL)
                                BEGIN
                                SET @SQL = @SQL + ' AND RT.CustomerID = @CustomerIDParameter'
                                END
                                IF(@Zone IS NOT NULL)
                                BEGIN
                                SET @SQL = @SQL+'  AND OZ.Name LIKE @ZoneParameter '
                                END
                                IF(@Code IS NOT NULL)
                                BEGIN
                                SET @SQL = @SQL+'  AND RT.Code LIKE @CodeParameter '
                                END
                                IF(@CustomerID IS NOT NULL)
                                BEGIN
                                SET @SQL = @SQL+ ' ORDER BY RT.Code '
                                END
                                ELSE
                                BEGIN
                                SET @SQL = @SQL+ ' ORDER BY RT.CustomerID, RT.Code '
                                END 
                                SET @SQL = @SQL+ ' SELECT COUNT(1) FROM #FirstTempTbl;
								  
WITH FINAL AS 
(
   SELECT *,ROW_NUMBER()  OVER ( ORDER BY CustomerID , Code) AS rowNumber
   FROM #FirstTempTbl
 )
SELECT * INTO #tmptbl FROM FINAL WHERE RowNumber BETWEEN '+CONVERT(VARCHAR(10),@From)+' AND '+CONVERT(VARCHAR(10),@To)+'
SET @Count = '+CONVERT(VARCHAR(10),@To)+'
SELECT * FROM #tmptbl
SET @Index = '+CONVERT(VARCHAR(10),@From)+'
WHILE( @Index <=@Count)
BEGIN

	SELECT @RouteIDVar = RouteID from #tmptbl WHERE rowNumber = @Index
	IF(@showBlocksParameter = ''Y'')
	BEGIN
	;WITH RouteOptionsCTE AS ( SELECT
                    RO.RouteID,
                    RO.SupplierID,
                    RO.SupplierZoneID,
                    RO.SupplierServicesFlag, 
                    RO.SupplierActiveRate,
                    RO.Priority,
                    RO.NumberOfTries,
                    RO.Percentage,
                    RO.State,
                    RN = Row_Number() OVER (PARTITION BY (RO.RouteID) ORDER BY RO.Priority DESC, RO.SupplierActiveRate ASC)
                FROM
                    RouteOption RO WITH(NOLOCK, index(IDX_RouteOption_RouteID))
                WHERE RO.RouteID = @RouteIDVar
                )
	INSERT INTO @RouteOptionTable
                SELECT RO.RouteID,
                    RO.SupplierID,
                    RO.SupplierZoneID,
                    RO.SupplierServicesFlag, 
                    RO.SupplierActiveRate,
                    RO.Priority,
                    RO.NumberOfTries,
                    RO.Percentage,
                    RO.State FROM RouteOptionsCTE RO WHERE RO.RN <= '+CONVERT(VARCHAR(10),@TopValues)+'
    END
    ELSE
    BEGIN
    	;WITH RouteOptionsCTE AS ( SELECT
                    RO.RouteID,
                    RO.SupplierID,
                    RO.SupplierZoneID,
                    RO.SupplierServicesFlag, 
                    RO.SupplierActiveRate,
                    RO.Priority,
                    RO.NumberOfTries,
                    RO.Percentage,
                    RO.State,
                    RN = Row_Number() OVER (PARTITION BY (RO.RouteID) ORDER BY RO.Priority DESC, RO.SupplierActiveRate ASC)
                FROM
                    RouteOption RO WITH(NOLOCK, index(IDX_RouteOption_RouteID))
                WHERE RO.RouteID = @RouteIDVar
                )
	INSERT INTO @RouteOptionTable
                SELECT RO.RouteID,
                    RO.SupplierID,
                    RO.SupplierZoneID,
                    RO.SupplierServicesFlag, 
                    RO.SupplierActiveRate,
                    RO.Priority,
                    RO.NumberOfTries,
                    RO.Percentage,
                    RO.State FROM RouteOptionsCTE RO WHERE RO.RN <= '+CONVERT(VARCHAR(10),@TopValues)+' AND RO.State = 1 
    END
                    
	SET @Index = @Index+1
	SET @RouteIDVar = 0
END


SELECT * FROM @RouteOptionTable'

print @SQL
--execute sp_executesql @SQL

EXECUTE sp_executesql 
         @SQL,
          N'@showBlocksParameter CHAR(1),@ZoneParameter VARCHAR(100) = NULL, @CodeParameter VARCHAR(15) = NULL,@SupplierIDParameter VARCHAR(5) = NULL, @CustomerIDParameter VARCHAR(5) = NULL',
          @showBlocksParameter = @showBlocks,
          @ZoneParameter = @zone,
          @CodeParameter = @Code,
          @SupplierIDParameter = @SupplierID,
          @CustomerIDParameter = @CustomerID;


END