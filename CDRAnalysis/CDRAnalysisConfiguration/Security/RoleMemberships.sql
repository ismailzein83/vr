EXECUTE sp_addrolemember @rolename = N'db_securityadmin', @membername = N'VANRISE\development';


GO
EXECUTE sp_addrolemember @rolename = N'db_owner', @membername = N'walid';


GO
EXECUTE sp_addrolemember @rolename = N'db_owner', @membername = N'VANRISE\wissam.ajamy';


GO
EXECUTE sp_addrolemember @rolename = N'db_owner', @membername = N'VANRISE\development';

