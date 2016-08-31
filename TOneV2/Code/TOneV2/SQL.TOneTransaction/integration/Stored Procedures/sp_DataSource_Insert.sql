-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSource_Insert]
	@Name varchar(100),
	@AdapterTypeId int,
	@AdapterState varchar(1000),
	@TaskId int,
	@Settings varchar(max),
	@Id int out
AS
BEGIN
	Insert into integration.DataSource ([Name], [AdapterID], [AdapterState], [TaskId], [Settings])
	Values(@Name, @AdapterTypeId, @AdapterState, @TaskId, @Settings)
	
	Set @Id = SCOPE_IDENTITY()
END