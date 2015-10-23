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
  FROM [FraudAnalysis].[Filter]
	
	SET NOCOUNT OFF;
END