
CREATE PROCEDURE [Retail_BE].[sp_Product_Insert]
	@Name nvarchar(255),
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN
IF NOT EXISTS(select 1 from [Retail_BE].Product where Name = @Name)
	BEGIN
		Insert into [Retail_BE].Product ([Name], [Settings])
		values(@Name, @Settings)
		
		SET @Id = SCOPE_IDENTITY()
	END
END