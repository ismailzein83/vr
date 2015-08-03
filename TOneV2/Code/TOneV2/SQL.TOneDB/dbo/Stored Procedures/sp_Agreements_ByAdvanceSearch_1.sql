-- =============================================
-- Author:		<Author,Rabih>
-- Create date: <Create Date,09/18/2013,>
-- Description:	<Description,Created for getting Agreements using Advance Search Criteria>
-- =============================================
CREATE PROCEDURE [dbo].[sp_Agreements_ByAdvanceSearch]
(
      @CarrierID NVARCHAR(5)=null,
   @Description NVARCHAR(100)=null,
   @FromBED DATETIME=null,
   @ToBED DATETIME=null,
   @FromEED DATETIME=null ,
   @ToEED DATETIME =null,
   @IsActive CHAR(1)=null,
   @IsWhole CHAR(1)=null,
   @DealType INT=null,
   @DealContract INT=null ,
   @ZoneID INT=null,
   @Top INT = 10,
   @From  int = 1,
   @To int =10,
   @WhereCondition NVARCHAR(250),
   @OrderByColumn NVARCHAR(50),
   @DigitAfterDot INT = 5,
   @IsEffective CHAR(1)=null
)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @DigitAfterDots INT
	DECLARE @SQLString nvarchar(4000)
	DECLARE @ShowNameSuffix nvarchar(1)
	SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like 'ShowNameSuffix')
	;WITH 
	CarrierTable as 
	(
		 SELECT 
		 ( CASE WHEN  @ShowNameSuffix ='Y' THEN (case when A.NameSuffix!='' THEN  P.Name+'('+A.NameSuffix+')' else P.Name end ) ELSE (P.Name ) END )  AS CarrierName 
		   ,A.CarrierAccountID FROM CarrierAccount A inner join CarrierProfile P ON P.ProfileID=A.ProfileID
		 )
		SELECT  bh.*,ca.CarrierName as carrierName,
		( CASE bh.AgreementType
              WHEN 0 THEN 'Gentlemen Agreement'
              WHEN 1 THEN 'Commitment Agreement'
         END ) AS AgreementName ,
         ( CASE bh.ContractType
              WHEN 0 THEN 'Balanced Amount'
              WHEN 1 THEN 'Balanced Duration'
              WHEN 2 THEN 'Un Balanced'
         END ) AS ContractName
        INTO #RESULT
		FROM BilateralAgreement bh 
		LEFT JOIN CarrierTable ca ON ca.CarrierAccountID= bh.CarrierID
		WHERE  
          bh.BeginDate BETWEEN ISNULL(@FromBED,bh.BeginDate) AND ISNULL(@ToBED,bh.BeginDate)  
		  AND bh.EndDate BETWEEN ISNULL(@FromEED,bh.EndDate) AND IsNull(@ToEED,bh.EndDate)
          AND bh.CarrierID =ISNULL(@CarrierID,bh.CarrierID)
          AND bh.IsActive = ISNULL(@IsActive,bh.IsActive)
          --AND bh.IsWhole = ISNULL(@IsWhole,BH.IsWhole)
          AND bh.IsEffective = ISNULL(@IsEffective,BH.IsEffective)
          AND bh.ContractType = ISNULL(@DealContract,bh.ContractType)
          AND bh.AgreementType = ISNULL(@DealType,bh.AgreementType)
          AND bh.[Description] LIKE ISNULL('%'+@Description+'%',bh.[Description]) 
  
	SET @SQLString = '	
	DECLARE @Digit INT
	SELECT t.*,ROUND(t.TotalDuration,' + CONVERT(VARCHAR(4),@DigitAfterDot) +') as TotalDurationRounded,ROW_NUMBER()  OVER ( ORDER BY '+@OrderByColumn+' ) AS rowNumber
	into #resultFINAL
	FROM #RESULT t WHERE 1=1
	 '+@WhereCondition+' 
	 ORDER BY '+@OrderByColumn+'
	SELECT COUNT(1) FROM #resultFINAL
	SELECT TOP ' + CONVERT(VARCHAR(4),@Top) +' * FROM #resultFINAL WHERE rowNumber between '+ CAST( @From AS varchar) +' AND  '+CAST( @To as varchar) 
	
	
	PRINT @SQLString
	EXECUTE sp_executesql @SQLString 
END