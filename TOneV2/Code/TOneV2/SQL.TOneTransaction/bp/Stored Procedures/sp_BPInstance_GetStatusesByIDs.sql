
CREATE PROCEDURE [bp].[sp_BPInstance_GetStatusesByIDs]
	@ProcessIDs varchar(1000)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


	DECLARE @ProcessIDsTable TABLE (ProcessID INT);

        IF (@ProcessIDs IS NOT NULL)
			BEGIN
				INSERT INTO @ProcessIDsTable (ProcessID)
				SELECT CONVERT(INT, ParsedString) FROM bp.[ParseStringList](@ProcessIDs);
			END



    SELECT [ID]
      ,[ExecutionStatus]
	FROM bp.[BPInstance] as bps WITH(NOLOCK)
	WHERE 
	(@ProcessIDs is NULL or  bps.ID in (SELECT ProcessID FROM @ProcessIDsTable)   ) 
	ORDER BY CreatedTime
END