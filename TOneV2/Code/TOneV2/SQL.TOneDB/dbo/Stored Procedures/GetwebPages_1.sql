
CREATE PROCEDURE [dbo].[GetwebPages]
AS
--select m.ItemID, NavigateURL, PermissionID PagePermission, p.ID PermissionID, p.Name PermissionName
--from websiteMenuItemPermission mp
--inner join websiteMenuItem m  on m.ItemId = mp.ItemID
--left join Permission p on p.id like mp.PermissionID+'/%' --mp.PermissionID = p.ID
--where NavigateURL not like 'javascript%'
--order by NavigateURL

select m.ItemID, NavigateURL, PermissionID PagePermission, p.ID PermissionID, p.Name PermissionName
from websiteMenuItem m
left join websiteMenuItemPermission mp  on m.ItemId = mp.ItemID
left join Permission p on mp.PermissionID = p.ID
where NavigateURL not like 'javascript%'
order by NavigateURL