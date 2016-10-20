-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSource_Insert]
		@Id uniqueidentifier ,
	@Name varchar(100),
	@AdapterTypeId uniqueidentifier,
	@AdapterState varchar(1000),
	@TaskId uniqueidentifier,
	@Settings varchar(max)

AS
BEGIN
	Insert into integration.DataSource (Id,[Name], [AdapterID], [AdapterState], [TaskId], [Settings])
	Values(@Id,@Name, @AdapterTypeId, @AdapterState, @TaskId, @Settings)
END