-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_RatePlan_InsertOrUpdateChanges]
	@OwnerType INT,
	@OwnerID INT,
	@Changes NVARCHAR(MAX),
	@Status INT
AS
BEGIN
	UPDATE TOneWhS_Sales.RatePlan
	SET [Changes] = @Changes
	WHERE OwnerType = @OwnerType AND OwnerID = @OwnerID AND [Status] = @Status
	
	IF @@ROWCOUNT = 0 BEGIN
		INSERT INTO TOneWhS_Sales.RatePlan (OwnerType, OwnerID, [Changes], [Status])
		VALUES (@OwnerType, @OwnerID, @Changes, @Status)
	END
END