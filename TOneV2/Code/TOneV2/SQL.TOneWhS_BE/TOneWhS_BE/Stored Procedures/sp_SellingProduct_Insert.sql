-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SellingProduct_Insert]
	@Name nvarchar(255),
	@SellingNumberPlanID int,
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN
SET @Id =0;
IF NOT EXISTS(select 1 from TOneWhS_BE.SellingProduct where Name = @Name)
	BEGIN
	Insert into TOneWhS_BE.SellingProduct([Name], SellingNumberPlanID, [Settings])
	Values(@Name, @SellingNumberPlanID, @Settings)
	
	Set @Id = SCOPE_IDENTITY()
	END
END