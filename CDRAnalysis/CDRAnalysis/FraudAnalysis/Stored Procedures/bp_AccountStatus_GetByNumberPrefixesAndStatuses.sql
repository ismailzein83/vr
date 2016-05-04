

CREATE PROCEDURE [FraudAnalysis].[bp_AccountStatus_GetByNumberPrefixesAndStatuses] 
@CaseStatusIDs varchar(100), 
@NumberPrefixes varchar(50) = null
AS
BEGIN

DECLARE @CaseStatusIDsTable TABLE (CaseStatusID INT);
DECLARE @NumberPrefixesTable TABLE (NumberPrefix VARCHAR(30));

        IF (@CaseStatusIDs IS NOT NULL)
			BEGIN
				INSERT INTO @CaseStatusIDsTable (CaseStatusID)
				SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@CaseStatusIDs);
			END
			
		IF (@NumberPrefixes IS NOT NULL)
			BEGIN
				INSERT INTO @NumberPrefixesTable (NumberPrefix)
				SELECT ParsedString FROM [FraudAnalysis].[ParseStringList](@NumberPrefixes);
			END	
		ELSE
			BEGIN
				INSERT INTO @NumberPrefixesTable (NumberPrefix)
				VALUES ('')
			END

	SELECT accStatus.[AccountNumber]
	FROM [FraudAnalysis].[AccountStatus] accStatus WITH (NOLOCK)
	JOIN @NumberPrefixesTable numPrefixes ON accStatus.AccountNumber LIKE numPrefixes.NumberPrefix + '%'
	WHERE	(@CaseStatusIDs is null or accStatus.[Status] in (SELECT CaseStatusID FROM @CaseStatusIDsTable)) 
		AND (accStatus.validTill is null OR accStatus.validTill >= GETDATE())
END