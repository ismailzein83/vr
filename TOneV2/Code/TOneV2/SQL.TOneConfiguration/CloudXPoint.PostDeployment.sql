/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
delete from [common].[extensionconfiguration] where ID in ('64152327-5DB5-47AE-9569-23D38BCB18CC','423064C2-ACE8-4D70-8CFF-CDAA1461DBBE','6D6EB5E6-E1F6-49FF-A78F-E3A255FBC320')
--[common].[extensionconfiguration]---------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('1EE51230-FE31-4D01-9289-0E27E24D3601','WhS_RouteSync_SwitchRouteSynchronizer_BuiltInIVSwitch','CloudXPoint Switch Synchronizer','WhS_RouteSync_SwitchRouteSynchronizer','{"Editor":"whs-routesync-builtinivswitch-swsync"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[extensionconfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);----------------------------------------------------------------------------------------------------end