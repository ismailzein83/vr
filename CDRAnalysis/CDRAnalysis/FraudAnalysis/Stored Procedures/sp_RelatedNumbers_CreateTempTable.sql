

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_RelatedNumbers_CreateTempTable]
AS
BEGIN
	SET NOCOUNT ON;

      IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[FraudAnalysis].[RelatedNumbers_temp]') AND type in (N'U'))
      DROP TABLE [FraudAnalysis].[RelatedNumbers_temp]
      
      CREATE TABLE [FraudAnalysis].[RelatedNumbers_temp](
		  [AccountNumber] [varchar](30) NOT NULL,
		  [RelatedNumbers] [varchar](max) NOT NULL
	  ) 
	  
	  ALTER TABLE [FraudAnalysis].[RelatedNumbers_temp] ADD PRIMARY KEY([AccountNumber])
END