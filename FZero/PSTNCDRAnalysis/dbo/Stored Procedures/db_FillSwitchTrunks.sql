CREATE PROCEDURE db_FillSwitchTrunks
AS
BEGIN

	declare @FullName nvarchar(255) 
	declare @Name nvarchar(255)
	declare @Direction int  
	declare @Switchid int  
	declare @F5 nvarchar(255)
	declare @RelatedSwitchId int 

declare @relatedTrunkname nvarchar(255) 
declare @trunkId int

DECLARE s CURSOR FOR 
select * from dbo.Importedtrunks where relatedswitchId is not null
OPEN s
FETCH NEXT FROM s INTO @FullName, @Name,@Direction,@Switchid,@F5,@RelatedSwitchId
WHILE @@FETCH_STATUS = 0
BEGIN
insert into Truncks 
Default Values
set @trunkId=@@IDENTITY
select @relatedTrunkname= Name from SwitchProfiles where ID=@Switchid

INSERT INTO SwitchTruncks
           ([SwitchId] ,[TrunckId] ,[Name] ,[FullName] ,[DirectionId])
     VALUES(@Switchid  ,@trunkId ,@Name ,@FullName,@Direction)

INSERT INTO SwitchTruncks
           ([SwitchId] ,[TrunckId] ,[Name] ,[FullName] ,[DirectionId])
          VALUES (@RelatedSwitchId ,@trunkId ,'Unknown' ,@relatedTrunkname ,case when @Direction=1 then 2 when @Direction=2 then 1 else 3 end )
    




FETCH NEXT FROM s INTO @FullName, @Name,@Direction,@Switchid,@F5,@RelatedSwitchId
end
CLOSE s
DEALLOCATE s



END