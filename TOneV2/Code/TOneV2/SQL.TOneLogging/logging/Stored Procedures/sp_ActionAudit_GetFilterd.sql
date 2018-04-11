-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [logging].[sp_ActionAudit_GetFilterd]
		@Top INT ,
	@UserIds nvarchar(max),
	@ModuleIds nvarchar(max),
	@ActionIds nvarchar(max),
	@EntityIds nvarchar(max),
	@objId nvarchar(max),
	@objName nvarchar(max),
	@FromTime datetime,
	@ToTime datetime =  null
AS

SET NOCOUNT ON;

	BEGIN
		 DECLARE @UserIDsTable TABLE (UserID int)
		 INSERT INTO @UserIDsTable (UserID)
		 select Convert(int, ParsedString) from [bp].[ParseStringList](@UserIds)

		 DECLARE @ModuleIDsTable TABLE (ModuleID int)
		 INSERT INTO @ModuleIDsTable (ModuleID)
		 select Convert(int, ParsedString) from [bp].[ParseStringList](@ModuleIds)

		 DECLARE @ActionIDsTable TABLE (ActionID int)
		 INSERT INTO @ActionIDsTable (ActionID)
		 select Convert(int, ParsedString) from [bp].[ParseStringList](@ActionIds)

		 DECLARE @EntityIDsTable TABLE (EntityID int)
		 INSERT INTO @EntityIDsTable (EntityID)
		 select Convert(int, ParsedString) from [bp].[ParseStringList](@EntityIds)

	  select TOP (@Top) [ID]
		  ,[UserID]
		  ,[ModuleID]
		  ,[EntityID]
		  ,[ActionID]
		  ,[ObjectName]
		  ,[ObjectID]
		  ,[UrlID]	  
		  ,[LogTime]
		  ,[ActionDescription]
	  FROM [logging].[ActionAudit] as ac

	  where (@UserIds is null   or ac.UserID in (select UserId from @UserIDsTable))
	  AND (@ModuleIds is null   or ac.ModuleID in (select ModuleID from @ModuleIDsTable))
	  AND(@ActionIds is null   or ac.ActionID in (select ActionID from @ActionIDsTable))
	  AND (@EntityIds is null   or ac.EntityID in (select EntityID from @EntityIDsTable))
	  AND   (@objName is null or ac.ObjectName like '%'+ @objName + '%')
	  AND   (@objId is null or ac.ObjectID= @objId)
	  AND  (ac.LogTime >= @FromTime AND (@ToTime  IS NULL OR  ac.LogTime <=  @ToTime))
	  ORDER BY ID DESC
	END
SET NOCOUNT OFF