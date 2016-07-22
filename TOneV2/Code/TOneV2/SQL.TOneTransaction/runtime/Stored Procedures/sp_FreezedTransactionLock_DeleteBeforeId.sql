-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE runtime.sp_FreezedTransactionLock_DeleteBeforeId
	@BeforeID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DELETE [runtime].[FreezedTransactionLock]
    WHERE ID <= @BeforeID
END