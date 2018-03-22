-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
		
CREATE PROCEDURE [TOneWhS_BE].[sp_SwitchConnectivity_Insert]
	@Name nvarchar(450),
	@SwitchID int,
	@CarrierAccountID int,
	@Settings nvarchar(MAX),
	@BED Datetime,
	@EED Datetime,
	@CreatedBy int,
	@LastModifiedBy int,
	@Id int out
AS
BEGIN
SET @id =0;
IF NOT EXISTS(SELECT 1 FROM TOneWhS_BE.SwitchConnectivity WHERE [Name] = @Name)
	BEGIN
		Insert into TOneWhS_BE.SwitchConnectivity([Name],CarrierAccountID,SwitchID,Settings ,BED,EED, CreatedBy, LastModifiedBy, LastModifiedTime)
		Values(@Name,@CarrierAccountID, @SwitchID, @Settings,@BED,@EED, @CreatedBy, @LastModifiedBy, GETDATE())
	
		Set @Id = SCOPE_IDENTITY()
	END
END