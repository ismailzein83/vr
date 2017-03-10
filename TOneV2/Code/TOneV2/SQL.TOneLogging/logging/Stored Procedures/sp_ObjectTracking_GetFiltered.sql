-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [logging].[sp_ObjectTracking_GetFiltered]
	(@LoggableEntityId uniqueidentifier ,
	@ObjectId varchar(255))
AS

SET NOCOUNT ON;

	BEGIN

	 SELECT ID,UserID
		  ,ActionID
		  ,LogTime
	  FROM [DemoProject_Dev_Log].[logging].[ObjectTracking] as ac

	  where ac.LoggableEntityID=@LoggableEntityId
	  AND ac.ObjectID=@ObjectId
	END
SET NOCOUNT OFF