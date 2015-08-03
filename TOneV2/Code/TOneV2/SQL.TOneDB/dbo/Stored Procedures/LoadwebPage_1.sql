CREATE PROCEDURE [dbo].[LoadwebPage] (@id int)
AS
--select m.ItemID, NavigateURL, PermissionID PagePermission, p.ID PermissionID, p.Name PermissionName
--from websiteMenuItemPermission mp
--inner join  websiteMenuItem m on m.ItemId = mp.ItemID
--left join Permission p on p.id like mp.PermissionID+'/%' --mp.PermissionID = p.ID
--where m.itemId = @id
--order by NavigateURL

select m.ItemID, NavigateURL, PermissionID PagePermission, p.ID PermissionID, p.Name PermissionName,p.PermissionLevel
from websiteMenuItem m
left join  websiteMenuItemPermission mp on m.ItemId = mp.ItemID
left join Permission p on p.id like mp.PermissionID+'/%' --mp.PermissionID = p.ID
where m.itemId = @id
order by NavigateURL