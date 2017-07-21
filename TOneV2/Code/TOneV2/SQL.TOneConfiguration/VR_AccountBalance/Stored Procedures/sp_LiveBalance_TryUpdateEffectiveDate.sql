-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_AccountBalance].[sp_LiveBalance_TryUpdateEffectiveDate] 
	@AccountID varchar(50),
	@AccountTypeID uniqueidentifier,
	@BED datetime ,
	@EED datetime
AS
BEGIN
IF  EXISTS(select 1 from [VR_AccountBalance].LiveBalance where AccountId = @AccountID and AccountTypeID =@AccountTypeID) 
	BEGIN
	UPDATE	[VR_AccountBalance].LiveBalance 
	SET		BED = @BED,
			EED = @EED
	WHERE	 AccountId = @AccountID and AccountTypeID =@AccountTypeID
	END
END