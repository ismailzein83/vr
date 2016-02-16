-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_OperatorProfile_Update]
	@ID int,
	@Name nvarchar(255),
	@Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(select 1 from dbo.OperatorProfile where Name = @Name and Id!=@ID) 
BEGIN
	Update dbo.OperatorProfile
	Set Name = @Name,
		Settings=@Settings
	Where ID = @ID
END
END