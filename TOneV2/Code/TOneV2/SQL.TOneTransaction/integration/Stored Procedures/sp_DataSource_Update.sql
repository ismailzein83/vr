-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSource_Update]
	@ID int,
	@Name varchar(100),
	@AdapterTypeId int,
	@Settings varchar(max)
AS
BEGIN
	Update integration.DataSource 
	Set Name = @Name,
		AdapterID = @AdapterTypeId,
		Settings = @Settings
	Where ID = @ID
END