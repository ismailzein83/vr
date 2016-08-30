CREATE PROCEDURE [reprocess].[sp_ReprocessDefinition_Insert]
	@Name Nvarchar(255),
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN
IF NOT EXISTS(select 1 from [reprocess].ReprocessDefinition where Name = @Name)
	BEGIN
		Insert into [reprocess].ReprocessDefinition ([Name], [Settings])
		values(@Name, @Settings)
		
		SET @Id = SCOPE_IDENTITY()
	END
END