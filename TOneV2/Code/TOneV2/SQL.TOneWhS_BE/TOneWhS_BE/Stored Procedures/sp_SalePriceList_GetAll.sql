-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePriceList_GetAll]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT	[ID],[OwnerID],[OwnerType],[CurrencyID], [EffectiveOn], [PriceListType], [ProcessInstanceID], [FileID]
from	[TOneWhS_BE].SalePriceList WITH(NOLOCK) 

END