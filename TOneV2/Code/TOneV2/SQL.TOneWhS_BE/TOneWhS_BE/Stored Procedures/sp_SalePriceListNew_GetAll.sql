-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePriceListNew_GetAll]
@ProcessInstanceId as bigint
AS
BEGIN
	set nocount on;

	select [ID], [OwnerID], [OwnerType], [CurrencyID], [EffectiveOn], [PriceListType], [ProcessInstanceID], [FileID], [CreatedTime],issent,SourceID,[UserID],[Description],[PricelistStateBackupID]
	from [TOneWhS_BE].SalePriceList_New with(nolock)
	where ProcessInstanceID = @ProcessInstanceId AND PriceListType != 3
END