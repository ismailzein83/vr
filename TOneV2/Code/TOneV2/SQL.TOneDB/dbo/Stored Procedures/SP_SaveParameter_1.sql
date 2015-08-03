
CREATE PROCEDURE [dbo].[SP_SaveParameter]
				@Name	VARCHAR(50),
				@Type TINYINT, 
				@BooleanValue CHAR(1) = NULL, 
				@NumericValue DECIMAL(20,8) = NULL,  
				@TimeSpanValue VARCHAR(50) = NULL, 
				@DateTimeValue DATETIME = NULL, 
				@TextValue NVARCHAR(max) = NULL, 
				@LongTextValue NTEXT = NULL, 
				@Description NTEXT = NULL, 
				@UserID INT = NULL
WITH RECOMPILE
AS
BEGIN	
	DECLARE @Existance INT	
	SET @Existance = (SELECT COUNT(*) FROM dbo.SystemParameter WITH(NOLOCK) WHERE Name LIKE @Name)
	IF(@Existance = 0)
		BEGIN 
			INSERT INTO dbo.SystemParameter (Name,[Type],BooleanValue,NumericValue,TimeSpanValue,DateTimeValue,TextValue,LongTextValue,[Description],UserID)
			VALUES(@Name,@Type,@BooleanValue,@NumericValue,@TimeSpanValue,@DateTimeValue,@TextValue,@LongTextValue,@Description,@UserID)
		END
	ELSE
		BEGIN 
			UPDATE dbo.SystemParameter
			SET [Type] = @Type,
			    BooleanValue = @BooleanValue,
			    NumericValue = @NumericValue,
			    TimeSpanValue = @TimeSpanValue,
			    DateTimeValue = @DateTimeValue,
			    TextValue = @TextValue,
			    LongTextValue = @LongTextValue,
			    [Description] = @Description,
			    UserID = @UserID
			WHERE Name = @Name
		END 
END