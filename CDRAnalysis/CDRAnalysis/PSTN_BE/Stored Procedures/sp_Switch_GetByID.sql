﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_Switch_GetByID]
	@SwitchID INT
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT s.ID,
		s.Name,
		s.TypeID,
		st.Name AS TypeName,
		s.AreaCode,
		s.TimeOffset,
		s.DataSourceID
	
	FROM PSTN_BE.Switch s
	INNER JOIN PSTN_BE.SwitchType st ON st.ID = s.TypeID
	
	WHERE s.ID = @SwitchID
	
	SET NOCOUNT OFF;
END