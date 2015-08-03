CREATE  FUNCTION [dbo].[GetTimePart] (@Date  datetime)  
RETURNS Char(5)  AS  
BEGIN 
	
	Return convert(varchar(5),@Date,108)
END