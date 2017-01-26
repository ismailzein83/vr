
Create PROCEDURE [Retail_BE].[sp_ProductFamily_Insert]
	@Name nvarchar(255),
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN
IF NOT EXISTS(select 1 from [Retail_BE].ProductFamily where Name = @Name)
	BEGIN
		Insert into [Retail_BE].ProductFamily ([Name], [Settings])
		values(@Name, @Settings)
		
		SET @Id = SCOPE_IDENTITY()
	END
END