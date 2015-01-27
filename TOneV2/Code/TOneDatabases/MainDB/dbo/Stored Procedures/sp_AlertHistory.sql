-- =============================================
-- Author:		<Author,Rabih>
-- Create date: <Create Date,09/18/2013,>
-- Description:	<Description,Created for getting Agreements using Advance Search Criteria>
-- =============================================
CREATE PROCEDURE [dbo].[sp_AlertHistory]
(
   @CarrierID NVARCHAR(10) = NULL,
   @AlertTitle NVARCHAR(100) = NULL,
   @Action nvarchar(50)= NULL,
   @AgrDescription NVARCHAR(100) = NULL,
   @Level int = NULL,
   @WhereCondition NVARCHAR(250) ,
   @OrderByColumn NVARCHAR(50) = ' ActionDate DESC',
   @From  int = 1,
   @To int =10,
   @FromDate DATETIME = NULL,
   @ToDate DATETIME = NULL,
   @AgreementID int = NULL
)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @SQLString nvarchar(4000)
	DECLARE @ShowNameSuffix nvarchar(1)
	SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like 'ShowNameSuffix')
	;WITH CarrierTable as 
	(
		 SELECT 
		 ( CASE WHEN  @ShowNameSuffix ='Y' THEN (case when A.NameSuffix!='' THEN  P.Name+'('+A.NameSuffix+')' else P.Name end ) ELSE (P.Name ) END )  AS CarrierName 
		   ,A.CarrierAccountID FROM CarrierAccount A inner join CarrierProfile P ON P.ProfileID=A.ProfileID
	)
	SELECT BM.ID AS MessageID,
	DATEADD(ms,-datepart(ms,BM.ActionDate),BM.ActionDate) AS ActionDate,
	BM.[Message],A.AlertID,BA.ID AS AgrID,A.[Level],A.[Action],
	C.CarrierName AS CarrierName ,
	ba.Description AS AgreDescription
	,A.Title,
	CASE [Level]                      
		 WHEN 0 THEN 'Critical'
		 WHEN 1 THEN 'Urgent'
		 WHEN 2 THEN 'High'
		 WHEN 3 THEN 'Medium'
		 WHEN 4 THEN 'Low'
    END AS LevelValue
	INTO #RESULT
	FROM BilateralAlertingMessage BM
	LEFT JOIN Bilateral_Alert A ON BM.AlertID = A.AlertID
	LEFT JOIN Bilateral_Agreement BA ON BA.ID = BM.AgreementID
	LEFT JOIN CarrierTable C ON C.CarrierAccountID=BA.CarrierID
	WHERE  
		 A.[Level] = ISNULL(@LEVEL,A.[Level]) 
		  AND  BA.[Description]  LIKE ISNULL('%'+@AgrDescription+'%',BA.[Description]) 
          AND A.Title LIKE A.Title
          AND A.[Action] LIKE ISNULL('%'+@Action+'%',A.[Action])
          AND  BA.CarrierID like ISNULL('%'+@CarrierID+'%',BA.CarrierID)
		  AND BM.ActionDate BETWEEN ISNULL(@FromDate,BM.ActionDate) AND ISNULL(@ToDate,bm.ActionDate)
		  AND BA.ID = ISNULL(@AgreementID,BA.ID)
	SET @SQLString = '
	
	SELECT t.*
    ,ROW_NUMBER()  OVER ( ORDER BY '+@OrderByColumn+' ) AS rowNumber
	INTO #resultFINAL
	FROM #RESULT t 
	WHERE 1=1
	 '+@WhereCondition+' 
	ORDER BY '+@OrderByColumn+'
	
	SELECT COUNT(1) FROM #resultFINAL
	SELECT * FROM #resultFINAL WHERE rowNumber between '+ CAST( @From AS varchar) +' AND  '+CAST( @To as varchar) 
	print @SQLString
	EXECUTE sp_executesql @SQLString 
END