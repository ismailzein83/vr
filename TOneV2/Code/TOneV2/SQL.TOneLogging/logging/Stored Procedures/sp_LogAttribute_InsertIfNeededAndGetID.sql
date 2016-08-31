
CREATE PROCEDURE [logging].[sp_LogAttribute_InsertIfNeededAndGetID]
	@AttributeType int,
	@Description varchar(1000)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @ID int
	
	SELECT @ID = ID
	FROM [logging].[LogAttribute]
	WHERE AttributeType = @AttributeType AND Description = @Description
	
	IF @ID IS NULL
	BEGIN
		INSERT INTO [logging].[LogAttribute]
           ([AttributeType]
           ,[Description])
		VALUES
		   (@AttributeType
		   ,@Description)
		SET @ID = SCOPE_IDENTITY()
	END
	
	SELECT @ID
	
END