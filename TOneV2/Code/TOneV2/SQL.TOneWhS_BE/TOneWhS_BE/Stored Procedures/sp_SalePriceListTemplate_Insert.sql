-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_BE.sp_SalePriceListTemplate_Insert
	@Name nvarchar(255),
	@Settings nvarchar(max),
	@Id int out
AS
BEGIN
	if not exists (select 1 from [TOneWhS_BE].[SalePriceListTemplate] where [Name] = @Name)
	begin
		insert into [TOneWhS_BE].[SalePriceListTemplate] ([Name], [Settings])
		values (@Name, @Settings)
		set @Id = scope_identity()
	end
END