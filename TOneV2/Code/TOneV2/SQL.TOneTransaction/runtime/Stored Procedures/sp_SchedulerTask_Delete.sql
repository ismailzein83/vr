﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_SchedulerTask_Delete]
	@Id INT
AS
BEGIN
	DELETE FROM [runtime].[ScheduleTask]
	WHERE ID = @Id
END