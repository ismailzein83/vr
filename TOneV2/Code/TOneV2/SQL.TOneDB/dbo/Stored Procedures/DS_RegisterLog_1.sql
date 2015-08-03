create procedure [dbo].[DS_RegisterLog] @SessionID BigInt , @LogPhrase varchar(300) , @d1 datetime , @d2 datetime
as
declare @SecondsSinceStart int
select @SecondsSinceStart = DATEDIFF( s, @d1, @d2 )
Insert into DS_LogProcess (SessionID, LogPhrase , SecondsSinceStart )
values
(@SessionID , @LogPhrase , @SecondsSincestart )