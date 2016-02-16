-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_OperatorProfile_Insert]
	@Name nvarchar(255),
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN
IF NOT EXISTS(select 1 from  dbo.OperatorProfile where Name = @Name)
BEGIN
	Insert into dbo.OperatorProfile([Name],[Settings])
	Values(@Name,@Settings)
	
	Set @Id = @@IDENTITY
	END
END