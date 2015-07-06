




CREATE PROCEDURE [dbo].[prGetRepeatedCases]
(
  @MobileOperatorID int = NULL,
  @ClientID int = NULL
)
AS
BEGIN  

declare @Query varchar(max)



							  	 
		set @Query =         '	select   rc.MobileOperatorID , rc.CLI, max(gc.AttemptDateTime) LastAttemptDateTime, max(gc.ID) as ID ,GETDATE() as FirstAttemptDateTime 
                                into #Temp
                               from generatedcalls gc
							  inner join recievedcalls rc on gc.ID=rc.GeneratedCallID
							  where gc.AttemptDateTime > 
							  (
							  
							    SELECT DATEADD(hour,24,max (r.SentDateTime)) FROM GeneratedCalls gc1 
							    inner JOIN dbo.RecievedCalls rc1 ON gc1.ID = rc1.GeneratedCallID
							    inner join Reports r on r.ID=gc1.ReportID
							     WHERE   
							    gc1.ReportingStatusID = 2 aNd  rc1.ClientID='+convert(varchar(10), @ClientID)+' and   rc1.MobileOperatorID='+convert(varchar(10), @MobileOperatorID) +'
							    And rc.CLI=rc1.CLI
							  					   
							   )
							   
					    	     anD gc.reportingstatusid=3 and rc.clientid='+convert(varchar(10), @ClientID)+' and   rc.MobileOperatorID='+convert(varchar(10), @MobileOperatorID) +'
					    	   group by rc.CLI,rc.MobileOperatorID order by LastAttemptDateTime desc 
								update t
								set t.FirstAttemptDateTime=
								(
								 select 
								 min(gc2.AttemptDateTime)
								 from GeneratedCalls gc2  inner JOIN dbo.RecievedCalls rc2 ON gc2.ID = rc2.GeneratedCallID
								  inner join Reports r on r.ID=gc2.ReportID
								 where rc2.cli=T.cli and gc2.ReportingStatusID = 2 and  rc2.ClientID='+convert(varchar(10), @ClientID)+' and   rc2.MobileOperatorID='+convert(varchar(10), @MobileOperatorID) +' 
								)
								from #Temp t 
								
								select * from #Temp  order by FirstAttemptDateTime
								drop table #Temp
							  	 '

                 
    
	
			set @Query=@Query +' '

      exec (@Query)                
                      
                      

END