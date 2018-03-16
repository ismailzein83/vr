-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [rules].[sp_Rule_Update]
	@ID INT,
	@TypeID INT,
	@RuleDetails VARCHAR(MAX),
	@BED Datetime,
	@EED Datetime,
	@LastModifiedBy int
AS
BEGIN
	Update rules.[Rule]
	Set  TypeID = @TypeID,
		 RuleDetails = @RuleDetails,
		 BED = @BED,
		 EED = @EED,
		 LastModifiedBy = @LastModifiedBy,
		 LastModifiedTime = GETDATE()
	WHERE ID  =@ID

END