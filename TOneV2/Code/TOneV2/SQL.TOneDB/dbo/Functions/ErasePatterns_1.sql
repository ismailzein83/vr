
CREATE FUNCTION [dbo].[ErasePatterns](
	@Array VARCHAR(8000),
	@Separator CHAR(1),
	@Patterns VARCHAR(8000)
)
RETURNS VARCHAR(8000)
AS
BEGIN
   
   	DECLARE @SupplierID TABLE(CarrierAccountID VARCHAR(5))
   	INSERT INTO @SupplierID ( CarrierAccountID )
   	SELECT value AS CarrierAccountID
   	FROM ParseArray(@Patterns,@Separator)
		
	DECLARE @Result VARCHAR(8000) 
	SELECT @Result = COALESCE(@Result + '|', '') + value 
	FROM ParseArray(@Array,@Separator) 
	WHERE NOT EXISTS
        (
        SELECT  NULL
        FROM    @SupplierID
        WHERE   value LIKE '%' + CarrierAccountID + '%'
        )
	
    RETURN @Result
END