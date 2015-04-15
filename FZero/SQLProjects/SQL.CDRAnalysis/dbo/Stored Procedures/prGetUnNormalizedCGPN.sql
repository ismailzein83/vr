



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[prGetUnNormalizedCGPN]
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

    set @Query =         '  select distinct ''MSISDN'' Party, LEFT(cgpn,3) as Prefix,LEN(cgpn)as Length,in_trunk as TrunckName,SUM(durationinseconds/60) as CallsDurations, COUNT(*) as CallsCount from cdr
	where isnormalized=1 and A_Temp is null  and SourceID  ' ++cast(@SwitchID as varchar(20))
         
    
    
    
    if ( (@FromDate is not null) and  (@ToDate is not null))
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
    
      
    
    set @Query=@Query +' group by LEFT(cgpn,3),LEN(cgpn),in_trunk  order by COUNT(*) desc '
     
     
    
    
      exec (@Query)
	
	
	
	
	
	
END