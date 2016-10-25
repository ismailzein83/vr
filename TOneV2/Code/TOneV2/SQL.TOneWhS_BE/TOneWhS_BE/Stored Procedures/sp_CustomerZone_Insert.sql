-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CustomerZone_Insert]
	@customerId INT,
	@zones VARCHAR(MAX),
	@startEffectiveTime DATETIME,
	@id INT OUT
AS
BEGIN
	INSERT INTO TOneWhS_BE.CustomerZone (CustomerID, Details, BED)
	VALUES (@customerId, @zones, @startEffectiveTime)
	
	SET @id = SCOPE_IDENTITY()
END