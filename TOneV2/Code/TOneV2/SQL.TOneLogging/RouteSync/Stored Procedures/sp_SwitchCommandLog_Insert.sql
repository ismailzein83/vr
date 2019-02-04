-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [RouteSync].[sp_SwitchCommandLog_Insert]
	@ProcessInstanceID bigint,
	@SwitchId varchar(20),
	@Command varchar(max),
	@Response varchar(max),
	@Id bigint out
AS
BEGIN
	INSERT INTO [RouteSync].[SwitchCommandLog] ([ProcessInstanceID],[SwitchId],[Command],[Response])
 	VALUES (@ProcessInstanceID, @SwitchId, @Command, @Response)

	SET @Id = SCOPE_IDENTITY()
END