-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_Pop_Update]
	@ID int,
	@Name nvarchar(255),
	@Description nvarchar(MAX),
	@Quantity int,
	@Location nvarchar(max)
AS
BEGIN
IF NOT EXISTS(select 1 from dbo.Pop where Name = @Name and Id!=@ID) 
BEGIN
	Update dbo.Pop
	Set Name = @Name,
		Description = @Description,
		Quantity =@Quantity,
		Location =@Location
		
	Where ID = @ID
END

END