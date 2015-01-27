CREATE  FUNCTION [dbo].[GetTimePart] (@Date  datetime)  
RETURNS Char(5)  AS  
BEGIN 
	Declare @Hour 	char(2)
	Declare @Minute char(2)
	Declare @Time 	char(5)
	Set @Hour=convert(varchar(2),datepart(hh,@Date))
	if len(@Hour)=1
		Set @Hour='0'+@Hour
	Set @Minute=convert(char(2),datepart(n,@Date))
	if len(@Minute)=1
		Set @Minute='0'+@Minute
	Set @Time =@Hour+':'+@Minute
	Return (@Time)
END