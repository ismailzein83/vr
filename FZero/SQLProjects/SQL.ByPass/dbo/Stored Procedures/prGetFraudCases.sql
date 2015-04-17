


















CREATE PROCEDURE [dbo].[prGetFraudCases]
(
  
  @MobileOperatorID int = NULL,
  @FromAttemptDateTime DATETIME = NULL,
  @ToAttemptDateTime DATETIME = NULL,
  @ClientID int = NULL,
  @OnnetorOffnet int = NULL, --0: All, 1:Onnet, 2:ofnet
  @IsAdmin bit=NULL
)
AS
BEGIN  

declare @Query varchar(max)

      if (@IsAdmin=0) 
		 Begin            
                      
		set @Query =         'SELECT   distinct  COUNT(GeneratedCalls.ID) AS Occurance, ''Case'' + CONVERT(char(20), MAX(GeneratedCalls.ID)) AS CaseID, (CASE WHEN GeneratedCalls.OriginationNetwork IS NULL 
							  THEN ''INTERNATIONAL'' WHEN GeneratedCalls.OriginationNetwork = '''' THEN ''INTERNATIONAL'' ELSE GeneratedCalls.OriginationNetwork END) as OriginationNetwork  , (CASE WHEN Carriers.Name IS NULL THEN ''Route '' + RIGHT(''000'' + CONVERT(VARCHAR(4),   case when   GeneratedCalls.Type =''GSM'' then 5 else 201 end   ) , 4) WHEN Carriers.Name = '''' THEN ''Route '' + RIGHT(''000'' + CONVERT(VARCHAR(4),   case when   GeneratedCalls.Type =''GSM'' then 5 else 201 end   ) , 4) ELSE ''Route '' + RIGHT(''000'' + CONVERT(VARCHAR(4), Carriers.ID), 4) END) AS Carrier, 
							  MAX(GeneratedCalls.AttemptDateTime) AS LastAttemptDateTime, MIN(GeneratedCalls.AttemptDateTime) AS FirstAttemptDateTime, MAX(GeneratedCalls.b_number) 
							  AS b_number, RecievedCalls.CLI, MAX(GeneratedCalls.ID) AS ID, MAX(GeneratedCalls.MobileOperatorID) AS MobileOperatorID
		FROM         
							  GeneratedCalls INNER JOIN
							  Carriers ON isnull(GeneratedCalls.Carrier,'''') = isnull(Carriers.Name,'''') LEFT OUTER JOIN
							  Users AS Users_1 RIGHT OUTER JOIN
							  ApplicationUsers AS ApplicationUsers_1 ON Users_1.ID = ApplicationUsers_1.UserID ON GeneratedCalls.AssignedTo = ApplicationUsers_1.ID inner JOIN
							  RecievedCalls ON GeneratedCalls.ID = RecievedCalls.GeneratedCallID
		WHERE     (1 = 1) AND (GeneratedCalls.StatusID = 2) '
		End

	if (@IsAdmin=1) 
		 Begin
		set @Query =         'SELECT    COUNT(GeneratedCalls.ID) AS Occurance, ''Case'' + CONVERT(char(20), MAX(GeneratedCalls.ID)) AS CaseID, 
		GeneratedCalls.OriginationNetwork, GeneratedCalls.Carrier, 
							  MAX(GeneratedCalls.AttemptDateTime) AS LastAttemptDateTime, MIN(GeneratedCalls.AttemptDateTime) AS FirstAttemptDateTime, MAX(GeneratedCalls.b_number) 
							  AS b_number, RecievedCalls.CLI, MAX(GeneratedCalls.ID) AS ID, MAX(GeneratedCalls.MobileOperatorID) AS MobileOperatorID
		FROM         GeneratedCalls LEFT OUTER JOIN
							  Users AS Users_1 RIGHT OUTER JOIN
							  ApplicationUsers AS ApplicationUsers_1 ON Users_1.ID = ApplicationUsers_1.UserID ON GeneratedCalls.AssignedTo = ApplicationUsers_1.ID inner JOIN
							  RecievedCalls ON GeneratedCalls.ID = RecievedCalls.GeneratedCallID
		WHERE     (1 = 1) AND (GeneratedCalls.StatusID = 2)  '
      End
    
	 if  (@OnnetorOffnet =1)
    begin
      set @Query=@Query +' and  dbo.GeneratedCalls.MobileOperatorID = RecievedCalls.MobileOperatorID '
    end 

	 if  (@OnnetorOffnet =2)
    begin
      set @Query=@Query +' and  dbo.GeneratedCalls.MobileOperatorID <> RecievedCalls.MobileOperatorID '
    end 

    
    if ( (@FromAttemptDateTime is not null) and  (@ToAttemptDateTime is not null))
    begin
      set @Query=@Query +' and  (dbo.GeneratedCalls.AttemptDateTime between  '''+cast(@FromAttemptDateTime as varchar(20))+''' and ''' +cast(@ToAttemptDateTime as varchar(20))+''' )'
    end 
    
    
     if ( (@FromAttemptDateTime is not null) and  (@ToAttemptDateTime is  null))
    begin
      set @Query=@Query +' and  dbo.GeneratedCalls.AttemptDateTime >  '''+cast(@FromAttemptDateTime as varchar(20))+''' '
    end 
    
    
     if ( (@FromAttemptDateTime is  null) and  (@ToAttemptDateTime is not null))
    begin
      set @Query=@Query +' and  dbo.GeneratedCalls.AttemptDateTime < '''+cast(@ToAttemptDateTime as varchar(20))+''' '
    end 
    
    
     if (@MobileOperatorID <>0) 
    begin
      set @Query=@Query +' and   dbo.GeneratedCalls.MobileOperatorID='+convert(varchar(10), @MobileOperatorID)
    end 
    
     if (@ClientID <>0) 
    begin
    set @Query=@Query +' and   dbo.RecievedCalls.ClientID='+convert(varchar(10), @ClientID)
     end 
     
	 if (@IsAdmin=0) 
		 Begin
			set @Query=@Query +' GROUP BY RecievedCalls.CLI, Carriers.ID,GeneratedCalls.OriginationNetwork, ''Route '' + RIGHT(''00'' + CONVERT(VARCHAR(3), Carriers.ID), 3), GeneratedCalls.Type,Carriers.Name order by LastAttemptDateTime desc'
		 End
    

	 if (@IsAdmin=1) 
		 Begin
			set @Query=@Query +' GROUP BY RecievedCalls.CLI, GeneratedCalls.OriginationNetwork, GeneratedCalls.Carrier order by LastAttemptDateTime desc'
		 End


      exec (@Query)                
                      
                      

END