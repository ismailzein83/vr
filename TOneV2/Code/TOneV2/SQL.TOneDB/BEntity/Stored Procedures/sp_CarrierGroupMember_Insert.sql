-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_CarrierGroupMember_Insert] 
(
@CarrierGroupID smallint,
@CarrierAccountID varchar(5)
)
AS
BEGIN
	INSERT INTO BEntity.CarrierGroupMember
		 (	CarrierGroupID ,
			CarrierAccountID)
			VALUES
			( @CarrierGroupID,
			  @CarrierAccountID
			)
END