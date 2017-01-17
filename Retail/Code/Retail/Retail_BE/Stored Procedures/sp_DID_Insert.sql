
CREATE PROCEDURE [Retail_BE].[sp_DID_Insert]
	@Number varchar(50),
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN
IF NOT EXISTS(select 1 from [Retail_BE].DID where Number = @Number)
	BEGIN
		Insert into [Retail_BE].DID ([Number], [Settings])
		values(@Number, @Settings)
		
		SET @Id = SCOPE_IDENTITY()
	END
END