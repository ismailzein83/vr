-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE runtime.sp_FreezedTransactionLock_Insert
	@TransactionLockItemIds varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT runtime.FreezedTransactionLock
    (TransactionLockItemIds)
    VALUES
    (@TransactionLockItemIds)
END