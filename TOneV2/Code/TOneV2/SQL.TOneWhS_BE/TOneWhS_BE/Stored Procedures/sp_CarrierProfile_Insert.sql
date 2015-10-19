-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_BE].[sp_CarrierProfile_Insert]
	@Name nvarchar(255),
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN

	Insert into TOneWhS_BE.CarrierProfile([Name],[Settings])
	Values(@Name,@Settings)
	
	Set @Id = @@IDENTITY
END