CREATE PROCEDURE [Retail_BE].[sp_CreditClass_Insert]
	@Name Nvarchar(255),
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN
IF NOT EXISTS(select 1 from [Retail_BE].CreditClass where Name = @Name)
	BEGIN
		Insert into [Retail_BE].CreditClass ([Name], [Settings])
		values(@Name, @Settings)
		
		SET @Id = SCOPE_IDENTITY()
	END
END