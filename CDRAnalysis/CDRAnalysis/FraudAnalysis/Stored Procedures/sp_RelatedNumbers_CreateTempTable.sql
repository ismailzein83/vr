

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_RelatedNumbers_CreateTempTable]
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[FraudAnalysis].[RelatedNumbers_old]') AND type in (N'U'))
      DROP TABLE [FraudAnalysis].[RelatedNumbers_old]

      IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[FraudAnalysis].[RelatedNumbers_temp]') AND type in (N'U'))
      DROP TABLE [FraudAnalysis].[RelatedNumbers_temp]
      
      
      IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[FraudAnalysis].[PK_RelatedNumbers]') )
      ALTER TABLE [FraudAnalysis].[RelatedNumbers] DROP CONSTRAINT [PK_RelatedNumbers];

      CREATE TABLE [FraudAnalysis].[RelatedNumbers_temp](
      [AccountNumber] [varchar](30) NOT NULL,
	  [RelatedNumbers] [varchar](max) NOT NULL,
      CONSTRAINT [PK_RelatedNumbers] PRIMARY KEY CLUSTERED 
     ([AccountNumber] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END