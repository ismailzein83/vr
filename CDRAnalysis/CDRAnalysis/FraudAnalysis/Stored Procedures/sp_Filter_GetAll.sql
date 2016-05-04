CREATE PROCEDURE [FraudAnalysis].[sp_Filter_GetAll]
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT [ID]
      ,[Abbreviation]
      ,[OperatorTypeAllowed]
      ,[Description]
      ,[Label]
      ,[ToolTip]
      ,[ExcludeHourly]
      ,[CompareOperator]
      ,[MinValue]
      ,[MaxValue]
      ,[DecimalPrecision]
  FROM [FraudAnalysis].[Filter] WITH (NOLOCK)
	
	SET NOCOUNT OFF;
END