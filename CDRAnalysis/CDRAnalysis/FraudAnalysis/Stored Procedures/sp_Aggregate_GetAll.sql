CREATE PROCEDURE [FraudAnalysis].[sp_Aggregate_GetAll]
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT [ID]
      ,[Name]
      ,[OperatorTypeAllowed]
      ,[NumberPrecision]
  FROM [FraudAnalysis].[Aggregate]  WITH (NOLOCK)
	
	SET NOCOUNT OFF;
END