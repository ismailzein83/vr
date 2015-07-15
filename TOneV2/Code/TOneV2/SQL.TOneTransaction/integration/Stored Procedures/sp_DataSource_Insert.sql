-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSource_Insert]
	@AdapterTypeId int,
	@Name varchar(100),
	@TaskId int,
	@Settings varchar(max),
	@Id int out
AS
BEGIN
	Insert into integration.DataSource ([AdapterID], [Name], [TaskId], [Settings])
	Values(@AdapterTypeId, @Name, @TaskId, @Settings)
	
	Set @Id = @@IDENTITY
END