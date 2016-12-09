-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePriceList_GetAll]

AS
BEGIN
	set nocount on;

	select [ID], [OwnerID], [OwnerType], [CurrencyID], [EffectiveOn], [PriceListType], [ProcessInstanceID], [FileID], [CreatedTime]
	from [TOneWhS_BE].SalePriceList with(nolock)
END