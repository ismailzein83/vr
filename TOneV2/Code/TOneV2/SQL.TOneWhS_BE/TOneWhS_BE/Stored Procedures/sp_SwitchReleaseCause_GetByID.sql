﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SwitchReleaseCause_GetByID]
	@ID INT
AS
BEGIN
	SELECT ReleaseCode,ID
	FROM SwitchReleaseCause
	WHERE ID=@ID
END