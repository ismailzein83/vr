-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Ext_RebuildPrepaidMonthlyBillingTotals @Days int
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	declare @FromCallDate datetime
	declare @ToCallDate datetime

	set @FromCallDate =dbo.DateOf(dateadd(d,-@Days,getdate()))
	set @ToCallDate =dbo.DateOf(getdate())

	Exec [dbo].[bp_PrepaidDailyTotalUpdate]	@FromCallDate , @ToCallDate 

END