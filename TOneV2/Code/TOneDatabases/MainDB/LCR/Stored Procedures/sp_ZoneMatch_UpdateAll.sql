-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [LCR].[sp_ZoneMatch_UpdateAll]
	@IsFuture bit = 0
AS
BEGIN	
	IF @IsFuture = 0
	BEGIN
		WITH newZoneMatch AS (SELECT DISTINCT OC.SupplierZoneID OurZoneID, SC.SupplierZoneID SupplierZoneID, SC.SupplierID
							  FROM LCR.CodeMatchCurrent_temp OC WITH(NOLOCK), LCR.CodeMatchCurrent_temp SC WITH(NOLOCK)
							  WHERE 
									OC.Code = SC.Code 
									AND OC.SupplierID = 'SYS'
									AND SC.SupplierID <> 'SYS'
							 )
		
		INSERT INTO LCR.ZoneMatchCurrent_temp
			   ([OurZoneID]
			   ,[SupplierZoneID]
			   ,[SupplierID])
		SELECT [OurZoneID]
			   ,[SupplierZoneID]
			   ,[SupplierID]
		FROM newZoneMatch 
	END
	ELSE
	BEGIN
		WITH newZoneMatch AS (SELECT DISTINCT OC.SupplierZoneID OurZoneID, SC.SupplierZoneID SupplierZoneID, SC.SupplierID
							  FROM LCR.CodeMatchFuture_temp OC WITH(NOLOCK), LCR.CodeMatchFuture_temp SC WITH(NOLOCK)
							  WHERE 
									OC.Code = SC.Code 
									AND OC.SupplierID = 'SYS'
									AND SC.SupplierID <> 'SYS'
							 )
		
		INSERT INTO LCR.ZoneMatchFuture_temp
			   ([OurZoneID]
			   ,[SupplierZoneID]
			   ,[SupplierID])
		SELECT [OurZoneID]
			   ,[SupplierZoneID]
			   ,[SupplierID]
		FROM newZoneMatch 
	END
END