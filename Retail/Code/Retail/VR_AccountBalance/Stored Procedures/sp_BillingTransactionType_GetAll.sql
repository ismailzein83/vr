-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE VR_AccountBalance.sp_BillingTransactionType_GetAll
AS
BEGIN
	SELECT ID, Name
	FROM VR_AccountBalance.BillingTransactionType
END