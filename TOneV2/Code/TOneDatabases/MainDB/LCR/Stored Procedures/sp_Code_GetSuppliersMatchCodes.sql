-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [LCR].[sp_Code_GetSuppliersMatchCodes]
	@Code varchar(30),
	@ActiveSupplierIDs LCR.SuppliersCodeInfoType READONLY,
	@EffectiveOn datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    declare @index int = Len(@code)


	declare @matchCodes table (code varchar(30))

	while (@index > 0)
	begin 
		insert into @matchCodes values (Substring(@code, 1, @index))
		select @index = @index - 1
	end;


	select  Z.SupplierID, c.Code, c.ID, C.ZoneID from @matchCodes mc 
	JOIN Code c WITH (NOLOCK) on mc.Code = c.Code
	JOIN Zone z WITH (NOLOCK) on c.ZoneID = z.ZoneID
	JOIN @ActiveSupplierIDs sup on z.SupplierID = sup.SupplierID
	where c.BeginEffectiveDate <= @EffectiveOn
		AND (c.EndEffectiveDate IS NULL OR c.EndEffectiveDate > @EffectiveOn)
	ORDER BY Z.SupplierID, c.Code desc
END