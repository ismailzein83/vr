Create PROCEDURE [TOneWhS_BE].[sp_Switch_Insert]
	@Name nvarchar(255),
	@Id int out
AS
BEGIN
IF NOT EXISTS(select 1 from  TOneWhS_BE.Switch where Name = @Name)
BEGIN
	Insert into TOneWhS_BE.Switch([Name])
	Values(@Name)
	
	Set @Id = @@IDENTITY
	END
END