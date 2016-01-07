
CREATE PROCEDURE [common].[sp_Type_InsertIfNotExistsAndGetID]
	@Type varchar(900)
AS
BEGIN	
	
	--INSERT Rule Type if not exists
	IF NOT EXISTS (SELECT NULL FROM common.[Type] WITH(NOLOCK) WHERE [Type] = @Type)
	BEGIN
		INSERT INTO common.[Type] ([Type])
		SELECT @Type WHERE NOT EXISTS (SELECT NULL FROM common.[Type] WHERE [Type] = @Type)
	END
	
	SELECT ID FROM common.[Type] WHERE [Type] = @Type
END