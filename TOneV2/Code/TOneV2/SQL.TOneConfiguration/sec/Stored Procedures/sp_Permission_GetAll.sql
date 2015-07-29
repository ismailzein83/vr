﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_Permission_GetAll] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT perm.[HolderType]
		,CASE WHEN HolderType = 0 THEN u.Name ELSE r.Name END AS HolderName
      ,perm.[HolderId]
      ,perm.[EntityType]
      ,perm.[EntityId]
      ,perm.[PermissionFlags]
    from sec.Permission perm
	LEFT JOIN sec.[Group] r on r.ID = perm.HolderId
	LEFT JOIN sec.[User] u on u.ID = perm.HolderId
END