Create PROCEDURE [TOneWhS_Deal].[sp_DealDetailedProgress_Delete]
	@DealDetailedProgressIds nvarchar(max)
AS
BEGIN
	declare @DDPTable as table (DealDetailedProgressId bigint)
	if (@DealDetailedProgressIds is not null) 
	begin 
		insert into @DDPTable 
		select convert(bigint, ParsedString)
		from [TOneWhS_Deal].[ParseStringList](@DealDetailedProgressIds) 
	end

	delete from [TOneWhS_Deal].[DealDetailedProgress]
	where id in (select DealDetailedProgressId from @DDPTable)
END