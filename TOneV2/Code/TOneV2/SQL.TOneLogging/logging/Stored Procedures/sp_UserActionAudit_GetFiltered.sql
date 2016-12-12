-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [logging].[sp_UserActionAudit_GetFilterd]
	@Top INT ,
	@UserIds nvarchar(max),
	@ModuleName varchar(50),
	@ControllerName varchar(50),
	@ActionName varchar(50),
	@BaseUrl varchar(100),
	@FromTime datetime,
	@ToTime datetime =  null
AS

SET NOCOUNT ON;

	BEGIN
		 DECLARE @UserIDsTable TABLE (UserID int)
		 INSERT INTO @UserIDsTable (UserID)
		 select Convert(int, ParsedString) from [bp].[ParseStringList](@UserIds)
	  select TOP (@Top) [ID]
		  ,[UserID]
		  ,[ModuleName]
		  ,[ControllerName]
		  ,[ActionName]
		  ,[BaseUrl]	  
		  ,[LogTime]
	  FROM [logging].[UserActionAudit] as ac

	  where (@UserIds is null   or ac.UserID in (select UserId from @UserIDsTable))
	  and   (@ModuleName is null or ac.ModuleName like '%'+ @ModuleName + '%')
	  and   (@ControllerName is null or ac.ControllerName like '%'+ @ControllerName + '%')
	  and   (@ActionName is null or ac.ActionName like '%'+@ActionName  + '%')
	  and   (@BaseUrl is null or ac.BaseUrl like '%'+ @BaseUrl + '%')
	  and   (ac.LogTime >= @FromTime)
	  and   (@ToTime is null or   ac.LogTime <=  @ToTime)
	  order by ac.ID desc


	END
SET NOCOUNT OFF