-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_BE.sp_SalePriceListTemplate_GetAll
AS
BEGIN
    select [ID], [Name], [Settings]
	from [TOneWhS_BE].[SalePriceListTemplate]
END