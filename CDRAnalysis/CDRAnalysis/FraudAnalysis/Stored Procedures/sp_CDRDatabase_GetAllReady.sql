
CREATE PROCEDURE [FraudAnalysis].[sp_CDRDatabase_GetAllReady]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [FromTime]
      ,[ToTime]
      ,[Settings]
      ,[CreatedTime]
      ,[timestamp]
  FROM [FraudAnalysis].[CDRDatabase]  WITH (NOLOCK)
  where IsReady = 1
END