-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_InsertCarrierInfoTest]
	@CarrierAccountID varchar(4),
	@Name varchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   INSERT INTO CarrierInfoTest (CarrierAccountID, Name) VALUES (@CarrierAccountID, @Name)
END