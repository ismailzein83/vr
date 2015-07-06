
CREATE PROCEDURE [dbo].[prDeleteEmail](@Ids varchar(4000))
AS

exec('Delete From msdb..sysmail_allitems where mailitem_id in (' + @Ids + ')')