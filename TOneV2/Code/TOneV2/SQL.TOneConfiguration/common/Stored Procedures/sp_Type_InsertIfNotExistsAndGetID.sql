
CREATE PROCEDURE [common].[sp_Type_InsertIfNotExistsAndGetID]
	@Type varchar(900)
AS
BEGIN	
	
	--INSERT Rule Type if not exists
	INSERT INTO common.[Type] WITH(TABLOCK) ([Type])
	SELECT @Type WHERE NOT EXISTS (SELECT NULL FROM common.[Type] WHERE [Type] = @Type)
	
	SELECT ID FROM common.[Type] WHERE [Type] = @Type
END