CREATE PROCEDURE [dbo].[GetHierarchicalPluginPermissions]
AS
select p1.ID, p1.Name, p2.ID ChildID, p2.Name ChildName
from PluginPermissions p1
inner join PluginPermissions p2 on  replace(p2.Id, p1.Id + '/', '') not Like '%/%'
where p1.Id !='' and p2.Id <> ''
Order by p1.id