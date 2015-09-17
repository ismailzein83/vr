




CREATE PROCEDURE [FraudAnalysis].[sp_AccountStatus_GetByStatuses] 

AS
BEGIN

SELECT accStatus.[AccountNumber]
      ,accStatus.[Status]
      ,accStatus.[AccountInfo]
  FROM [FraudAnalysis].[AccountStatus] accStatus
WHERE	accStatus.[Status] =1 or accStatus.[Status] =2 or accStatus.[Status] =3
END