



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[db_Dashboard]
	-- Add the parameters for the stored procedure here
	
	@Database varchar(50)=NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @Query varchar(max)
    set @Query =         ' use ['+@Database+']    declare @CountNewCDRs int; set @CountNewCDRs=0;set @CountNewCDRs =(select count (*) as NewCDRs from  CookedCDR where (isnormalized <>1 and a_temp is null and b_temp is null ))declare @MaxAttemptDateTime datetime; set @MaxAttemptDateTime=NULL; set @MaxAttemptDateTime= (select  AttemptDateTime from  CookedCDR where ID=(select  MAX(ID) from  CookedCDR)) ; declare @CountCGPN int; set @CountCGPN=0; set @CountCGPN=(select COUNT(*) CGPN from  CookedCDR where IsNormalized=1 and A_temp is null); declare @CountCDPN int; set @CountCDPN=0; set @CountCDPN=(select COUNT(*) CDPN from  CookedCDR where IsNormalized=1 and B_temp is null);  select @CountNewCDRs as CountNewCDRs, @MaxAttemptDateTime as MaxAttemptDateTime, @CountCDPN as CountCDPN , @CountCGPN  as CountCGPN;  '
    exec (@Query)
	
	
END