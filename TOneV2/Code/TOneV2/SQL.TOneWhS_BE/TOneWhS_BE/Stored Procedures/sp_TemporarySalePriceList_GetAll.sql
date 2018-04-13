-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_TemporarySalePriceList_GetAll]

AS
BEGIN
	set nocount on;

	select [ID], [OwnerID], [OwnerType], [CurrencyID], [EffectiveOn], [PriceListType], [ProcessInstanceID], [FileID], [CreatedTime],issent,SourceID,[UserID],[Description],[PricelistStateBackupID]
	from [TOneWhS_BE].SalePriceList_New with(nolock)
END