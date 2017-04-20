
CREATE Function [Mediation_Generic].[GetLegIdsSessionTable]  (@MediationDefinitionId uniqueidentifier, @SessionId varchar(200), @LegIds nvarchar(max) )  
Returns @tbl_string Table  (MediationDefinitionId uniqueidentifier, SessionId varchar(200), ParsedLegId nvarchar(max))  As  

BEGIN 

DECLARE @end Int,
        @start Int

SET @LegIds =  @LegIds + ',' 
SET @start=1
SET @end=1

WHILE @end<Len(@LegIds)
    BEGIN
        SET @end = CharIndex(',', @LegIds, @end)
        INSERT INTO @tbl_string 
            SELECT	@MediationDefinitionId,
					@SessionId,
					Substring(@LegIds, @start, @end-@start)

        SET @start=@end+1
        SET @end = @end+1
    END

RETURN
END