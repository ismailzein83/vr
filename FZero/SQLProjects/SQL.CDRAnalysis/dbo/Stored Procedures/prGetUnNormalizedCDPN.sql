﻿




-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[prGetUnNormalizedCDPN]
	-- Add the parameters for the stored procedure here
	@FromDate Datetime =NULL , 
	@ToDate Datetime =NULL , 
	@SwitchID int=NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	

	declare @Query varchar(max)

    set @Query =         '  select distinct ''Destination'' Party,   LEFT(CDPN,3)as Prefix,LEN(CDPN)as Length ,OUT_TRUNK as TrunckName,
    SUM(durationinseconds/60) as CallsDurations, COUNT(*) as CallsCount from cdr where  isnormalized=1 and B_Temp is null and SourceID  ' ++cast(@SwitchID as varchar(20))
         
    
    
    
    if ( (@FromDate <> null) and  (@ToDate is not null))
    begin
      set @Query=@Query +' and  (ConnectDateTime between  '''+cast(@FromDate as varchar(20))+''' and ''' +cast(@ToDate as varchar(20))+''' )'
    end 
    
    
     if ( (@FromDate is not null) and  (@ToDate is  null))
    begin
      set @Query=@Query +' and  ConnectDateTime >  '''+cast(@FromDate as varchar(20))+''' '
    end 
    
    
     if ( (@FromDate is  null) and  (@ToDate is not null))
    begin
      set @Query=@Query +' and  ConnectDateTime < '''+cast(@ToDate as varchar(20))+''' '
    end 
    
      
    
    set @Query=@Query +' group by LEFT(CDPN,3),LEN(CDPN),OUT_TRUNK order by COUNT(*) desc '
     
     
    
    
      exec (@Query)
	
	
	
	
	
	
END