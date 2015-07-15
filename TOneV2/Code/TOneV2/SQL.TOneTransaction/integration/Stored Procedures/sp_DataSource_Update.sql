-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSource_Update]
	@ID int,
	@AdapterTypeId int,
	@Name varchar(100),
	@Settings varchar(max)
AS
BEGIN
	Update integration.DataSource 
	Set AdapterID = @AdapterTypeId,
		Name = @Name,
		Settings = @Settings
	Where ID = @ID
END