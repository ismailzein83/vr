

CREATE PROCEDURE [FraudAnalysis].[sp_RelatedNumbers_SwapTableWithTemp]
AS
BEGIN
	BEGIN TRANSACTION
            IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[FraudAnalysis].[RelatedNumbers]') AND type in (N'U'))
            EXEC sp_rename 'FraudAnalysis.RelatedNumbers', 'RelatedNumbers_Old'
                  EXEC sp_rename 'FraudAnalysis.RelatedNumbers_Temp', 'RelatedNumbers'
    COMMIT TRANSACTION
    
     
      IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[FraudAnalysis].[RelatedNumbers_old]') AND type in (N'U'))
      DROP TABLE [FraudAnalysis].[RelatedNumbers_old]
    

END