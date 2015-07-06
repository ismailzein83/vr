
CREATE FUNCTION FindSwitchesTimeDiff
(
	@SwitchId1 int,
	@SwitchId2 int
)
RETURNS int
AS
BEGIN
	
	declare @value1 int
	declare @value2 int
	declare @result int
	
select @value1 = MinutesDiff from dbo.SwitchJadriahTimeDiff where SwitchId=@SwitchId1
select @value2 = MinutesDiff from dbo.SwitchJadriahTimeDiff where SwitchId=@SwitchId2

	if(@value1 > @value2)
	    begin
	      set @result= abs(@value1-@value2)
	    end
	else
	    begin
	       set @result= abs(@value2-@value1)
	    end

return @result
END