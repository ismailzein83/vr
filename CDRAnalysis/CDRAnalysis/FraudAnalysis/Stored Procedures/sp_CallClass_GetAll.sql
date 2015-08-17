




CREATE PROCEDURE [FraudAnalysis].[sp_CallClass_GetAll] 
AS
BEGIN
SELECT Id, Description, NetType FROM CallClass
END