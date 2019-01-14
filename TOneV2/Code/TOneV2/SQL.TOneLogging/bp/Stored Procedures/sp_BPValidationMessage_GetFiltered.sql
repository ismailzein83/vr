CREATE PROCEDURE [bp].[sp_BPValidationMessage_GetFiltered]
	@ProcessInstanceId BIGINT,
	@ArrSeverityID nvarchar(max)
AS
BEGIN
	SELECT [ID]
	  ,[ProcessInstanceID]
	  ,[ParentProcessID]
	  ,[TargetKey]
	  ,[TargetType]
	  ,[Severity]
	  ,[Message]
	FROM bp.[BPValidationMessage] WITH(NOLOCK)
	WHERE (@ArrSeverityID is NULL or Severity in (SELECT ParsedString FROM ParseStringList(@ArrSeverityID))) 
	And ProcessInstanceID = @ProcessInstanceId 
END