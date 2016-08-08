﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleCode_GetDistinctCodePrefixes]
	@PrefixLength int,
	@EffectiveOn DATETIME,
	@IsFuture bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;		
		SELECT LEFT(Code, @PrefixLength) as CodePrefix, 0 as codeCount FROM TOneWhS_BE.SaleCode WITH (NOLOCK) 
		WHERE
		(
			(@IsFuture = 0 AND BED <= @EffectiveOn AND (EED > @EffectiveOn OR EED IS NULL))
			OR
			(@IsFuture = 1 AND (BED > GETDATE() OR EED IS NULL))
		)
		group by LEFT(Code, @PrefixLength);
	SET NOCOUNT OFF;
END