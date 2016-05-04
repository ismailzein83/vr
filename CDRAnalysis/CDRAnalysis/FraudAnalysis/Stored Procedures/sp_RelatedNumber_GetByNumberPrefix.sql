
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_RelatedNumber_GetByNumberPrefix]
	@NumberPrefix varchar(30) = NULL
AS
BEGIN

	SELECT num.[AccountNumber]  ,num.[RelatedAccountNumber] FROM [FraudAnalysis].[RelatedNumber] num 
	where num.[AccountNumber] like @NumberPrefix + '%'
	
END