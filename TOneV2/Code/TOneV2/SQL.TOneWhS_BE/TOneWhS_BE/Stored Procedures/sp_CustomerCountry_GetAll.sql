-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_BE.sp_CustomerCountry_GetAll
	
AS
BEGIN
	select [ID], [CustomerID], [CountryID], [BED], [EED]
	from TOneWhS_BE.CustomerCountry
END