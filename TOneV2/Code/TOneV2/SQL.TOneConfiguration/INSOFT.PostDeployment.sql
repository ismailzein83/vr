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
--[common].[Setting]----------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
Update	s
SET		s.[Data] = REPLACE(s.[Data],'"ProductName":"T.One"','"ProductName":"INSOFT"')
from	[common].[Setting] s
where	ID='509E467B-4562-4CA6-A32E-E50473B74D2C'