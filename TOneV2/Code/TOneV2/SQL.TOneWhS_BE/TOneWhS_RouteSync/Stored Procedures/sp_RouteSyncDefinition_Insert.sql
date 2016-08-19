-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create Procedure [TOneWhS_RouteSync].[sp_RouteSyncDefinition_Insert]
	@Name nvarchar(255),
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN
	IF NOT EXISTS(select 1 from TOneWhS_RouteSync.RouteSyncDefinition where Name = @Name)
	BEGIN
		insert into TOneWhS_RouteSync.RouteSyncDefinition ([Name], [Settings])
		values(@Name, @Settings)
		
		set @Id = @@IDENTITY
	END
END