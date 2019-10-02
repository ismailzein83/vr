﻿/*
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

--[common].[VRDevProject]---------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[AssemblyID])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('646D738B-7344-4668-9E99-67FC2D6C18A2','ICX Ogero',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[AssemblyID]))
merge	[common].[VRDevProject] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[AssemblyID] = s.[AssemblyID]
when not matched by target then
	insert([ID],[Name],[AssemblyID])
	values(s.[ID],s.[Name],s.[AssemblyID]);


--[genericdata].[VRNumberPrefixType]----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Id],[DevProjectID],[Name])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D','646D738B-7344-4668-9E99-67FC2D6C18A2','Ogero'),
('C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC','646D738B-7344-4668-9E99-67FC2D6C18A2','Alfa'),
('1056D52E-DA10-4A15-B3A7-F073792F7D2A','646D738B-7344-4668-9E99-67FC2D6C18A2','Touch')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[DevProjectID],[Name]))
merge	[genericdata].[VRNumberPrefixType] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
--when matched then
--	update set
--	[Name] = s.[Name]
when not matched by target then
	insert([Id],[DevProjectID],[Name])
	values(s.[Id],s.[DevProjectID],s.[Name]);


--[genericdata].[VRNumberPrefix]--------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Number],[Type],[IsExact])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('31','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('32','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('33','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('34','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('35','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('031','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('032','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('033','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('034','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('035','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('701','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('702','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('703','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('704','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('705','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('710','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('716','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('717','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('718','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('719','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('761','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('763','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('764','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('765','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('811','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('812','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('813','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('814','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('815','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('102','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('103','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('104','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('105','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('791','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('7930','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('7931','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('0102','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('0103','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('0104','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('0105','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('79320','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('79321','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('79322','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('79323','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('79324','C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC',null),
('30','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('36','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('37','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('38','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('39','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('030','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('036','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('037','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('038','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('039','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('700','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('706','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('707','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('708','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('709','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('711','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('712','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('713','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('714','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('715','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('760','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('766','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('767','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('768','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('769','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('788','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('816','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('817','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('818','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('819','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('810','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('106','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('107','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('108','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('109','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('789','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('0106','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('0107','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('0108','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('0109','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('78970','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('78971','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('78972','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('78973','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('78974','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('78975','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('78976','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('78977','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('78978','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('78979','1056D52E-DA10-4A15-B3A7-F073792F7D2A',null),
('01','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',null),
('04','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',null),
('05','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',null),
('06','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',null),
('07','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',null),
('08','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',null),
('09','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',null),
('1','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',null),
('4','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',null),
('5','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',null),
('6','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',null),
('7','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',null),
('8','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',null),
('9','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',null),
('100','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('112','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('113','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('116','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('117','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('119','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('120','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('125','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('130','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('139','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('140','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('145','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('150','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('175','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('999','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1022','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1030','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1040','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1060','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1100','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1112','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1113','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1116','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1117','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1119','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1120','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1125','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1130','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1139','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1140','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1145','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1150','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1175','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1210','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1211','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1212','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1213','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1214','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1215','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1216','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1218','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1219','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1222','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1234','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1240','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1241','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1242','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1243','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1244','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1245','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1246','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1247','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1248','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1249','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1261','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1262','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1263','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1264','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1265','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1266','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1267','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1268','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1269','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1270','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1271','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1272','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1273','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1274','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1275','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1276','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1277','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1278','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1279','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1280','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1281','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1282','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1283','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1284','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1285','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1286','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1287','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1288','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1289','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1290','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1291','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1292','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1293','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1294','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1295','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1296','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1297','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1298','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1299','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1313','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1314','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1315','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1320','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1321','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1322','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1323','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1324','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1325','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1326','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1327','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1328','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1329','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1330','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1331','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1332','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1333','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1345','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1358','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1359','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1360','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1361','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1363','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1366','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1367','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1369','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1371','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1372','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1375','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1380','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1381','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1384','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1385','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1414','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1433','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1435','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1436','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1437','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1438','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1439','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1440','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1441','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1442','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1443','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1445','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1446','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1447','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1448','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1449','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1460','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1461','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1462','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1463','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1464','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1465','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1468','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1470','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1474','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1475','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1476','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1480','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1484','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1485','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1486','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1488','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1489','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1491','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1499','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1510','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1512','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1514','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1515','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1517','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1518','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1520','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1521','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1522','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1523','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1524','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1525','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1526','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1529','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1530','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1531','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1532','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1533','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1534','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1535','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1536','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1540','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1542','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1543','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1544','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1545','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1551','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1552','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1553','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1557','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1558','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1560','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1561','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1562','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1564','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1565','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1566','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1567','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1569','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1570','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1571','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1573','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1575','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1576','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1577','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1580','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1581','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1584','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1585','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1587','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1588','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1590','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1591','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1592','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1595','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1599','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1610','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1615','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1616','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1617','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1620','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1622','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1626','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1630','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1632','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1633','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1644','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1646','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1650','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1654','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1655','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1656','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1660','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1661','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1662','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1666','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1669','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1670','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1671','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1672','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1676','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1677','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1678','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1679','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1680','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1688','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1699','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1700','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1701','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1702','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1703','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1707','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1708','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1710','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1713','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1714','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1716','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1717','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1718','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1719','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1720','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1722','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1725','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1727','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1733','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1735','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1739','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1740','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1741','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1744','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1745','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1747','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1760','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1761','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1762','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1766','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1770','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1771','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1772','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1775','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1777','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1778','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1779','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1780','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1785','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1788','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1789','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1790','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1797','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1799','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1881','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('1999','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11010','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11011','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11022','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11030','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11040','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11060','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11078','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11080','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11099','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11210','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11211','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11212','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11213','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11214','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11215','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11216','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11218','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11219','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11222','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11234','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11240','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11241','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11242','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11243','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11244','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11245','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11246','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11247','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11248','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11249','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11261','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11262','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11263','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11264','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11265','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11266','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11267','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11268','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11269','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11270','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11271','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11272','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11273','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11274','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11275','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11276','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11277','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11278','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11279','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11280','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11281','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11282','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11283','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11284','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11285','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11286','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11287','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11288','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11289','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11290','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11291','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11292','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11293','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11294','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11295','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11296','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11297','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11298','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11299','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11313','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11314','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11315','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11320','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11321','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11322','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11323','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11324','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11325','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11326','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11327','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11328','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11329','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11330','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11331','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11332','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11333','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11345','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11358','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11359','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11360','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11361','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11363','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11366','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11367','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11369','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11371','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11372','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11375','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11380','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11381','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11384','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11385','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11414','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11433','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11435','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11436','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11437','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11438','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11439','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11440','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11441','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11442','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11443','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11445','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11446','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11447','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11448','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11449','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11460','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11461','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11462','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11463','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11464','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11465','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11468','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11470','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11474','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11475','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11476','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11480','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11484','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11485','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11486','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11488','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11489','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11491','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11499','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11510','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11512','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11514','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11515','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11517','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11518','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11520','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11521','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11522','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11523','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11524','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11525','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11526','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11529','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11530','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11531','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11532','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11533','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11534','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11535','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11536','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11540','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11542','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11543','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11544','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11545','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11551','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11552','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11553','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11557','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11558','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11560','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11561','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11562','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11564','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11565','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11566','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11567','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11569','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11570','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11571','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11573','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11575','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11576','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11577','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11580','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11581','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11584','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11585','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11587','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11588','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11590','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11591','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11592','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11595','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11599','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11610','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11615','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11616','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11617','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11620','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11622','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11626','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11630','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11632','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11633','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11644','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11646','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11650','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11654','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11655','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11656','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11660','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11661','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11662','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11666','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11669','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11670','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11671','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11672','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11676','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11677','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11678','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11679','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11680','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11688','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11699','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11700','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11701','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11702','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11703','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11707','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11708','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11710','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11713','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11714','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11716','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11717','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11718','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11719','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11720','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11722','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11725','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11727','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11733','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11735','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11739','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11740','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11741','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11744','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11745','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11747','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11760','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11761','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11762','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11766','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11770','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11771','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11772','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11775','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11777','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11778','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11779','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11780','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11785','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11788','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11789','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11790','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11797','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11799','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1),
('11881','FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Number],[Type],[IsExact]))
merge	[genericdata].[VRNumberPrefix] as t
using	cte_data as s
on		1=1 and t.[Number] = s.[Number]
when matched then
	update set
	[Number] = s.[Number],[Type] = s.[Type],[IsExact] = s.[IsExact]
when not matched by target then
	insert([Number],[Type],[IsExact])
	values(s.[Number],s.[Type],s.[IsExact]);