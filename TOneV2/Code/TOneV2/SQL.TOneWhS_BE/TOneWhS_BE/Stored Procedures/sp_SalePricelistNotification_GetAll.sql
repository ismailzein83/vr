-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePricelistNotification_GetAll]

AS
BEGIN

	BEGIN
			SELECT   pricelistId,EmailCreationDate,fileID
			FROM [TOneWhS_BE].[SalePricelistNotification] 
	
	END
END