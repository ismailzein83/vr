-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_BE.sp_SalePriceListTemplate_Update
	@Id int,
	@Name nvarchar(255),
	@Settings nvarchar(max)
AS
BEGIN
	if not exists (select 1 from [TOneWhS_BE].[SalePriceListTemplate] where [Name] = @Name and [ID] != @Id)
	begin
		update [TOneWhS_BE].[SalePriceListTemplate]
		set [Name] = @Name, [Settings] = @Settings
		where [ID] = @Id
	end
END