-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSource_Update]
	@ID int,
	@Name varchar(100),
	@AdapterTypeId int,
	@AdapterState varchar(1000),
	@Settings varchar(max)
AS
BEGIN
	Update integration.DataSource 
	Set Name = @Name,
		AdapterID = @AdapterTypeId,
		AdapterState = @AdapterState,
		Settings = @Settings
	Where ID = @ID
END