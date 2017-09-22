-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CarrierAccountStatusHistory_Insert]
	@CarrierAccountId int,
	@StatusID int,
	@PreviousStatusID int
AS
BEGIN
	insert into TOneWhS_BE.CarrierAccountStatusHistory([CarrierAccountID], [StatusID], [PreviousStatusID],[StatusChangedDate])
	values (@CarrierAccountId, @StatusID, @PreviousStatusID, GETDATE())
END