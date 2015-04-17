




CREATE PROCEDURE [dbo].[prGetFraudCases_LoadonDemand]
(
  
  @MobileOperatorID int = NULL,
  @FromAttemptDateTime DATETIME = NULL,
  @ToAttemptDateTime DATETIME = NULL,
  @ClientID int = NULL,
  @OnnetorOffnet int = NULL, --0: All, 1:Onnet, 2:ofnet
  @IsAdmin bit=NULL, 
  @PageIndex INT = 1, 
  @PageSize INT=30,
  @OnlyCount bit=NULL
)
AS
BEGIN  

declare @Query varchar(max)

      if (@IsAdmin=0) 
		 Begin            
                      
		set @Query =         'select s.*, ROW_NUMBER() OVER  (     ORDER BY s.LastAttemptDateTime desc  )AS RowNumber   into #Results   from (SELECT   distinct  COUNT(GeneratedCalls.ID) AS Occurance, ''Case'' + CONVERT(char(20), MAX(GeneratedCalls.ID)) AS CaseID, (CASE WHEN GeneratedCalls.OriginationNetwork IS NULL 
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
		set @Query =         'select s.*, ROW_NUMBER() OVER  (     ORDER BY s.LastAttemptDateTime desc  )AS RowNumber   into #Results   from (SELECT   distinct COUNT(GeneratedCalls.ID) AS Occurance, ''Case'' + CONVERT(char(20), MAX(GeneratedCalls.ID)) AS CaseID, 
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
			set @Query=@Query +' GROUP BY RecievedCalls.CLI, Carriers.ID,GeneratedCalls.OriginationNetwork, ''Route '' + RIGHT(''00'' + CONVERT(VARCHAR(3), Carriers.ID), 3), GeneratedCalls.Type,Carriers.Name ) s'
		 End
    

	 if (@IsAdmin=1) 
		 Begin
			set @Query=@Query +' GROUP BY RecievedCalls.CLI, GeneratedCalls.OriginationNetwork, GeneratedCalls.Carrier ) s'
		 End


         if (@OnlyCount=0) 
		 Begin 
				set @Query=@Query+'; DECLARE @RecordCount INT SELECT @RecordCount = COUNT(*) FROM #Results; DECLARE @PageCount INT SET @PageCount = CEILING(CAST(@RecordCount AS DECIMAL(10, 2)) / CAST('+convert(varchar(10), @PageSize)+' AS DECIMAL(10, 2)));        
									SELECT Occurance, CaseID, 	OriginationNetwork, Carrier, LastAttemptDateTime,  FirstAttemptDateTime,  b_number, CLI,  ID,  MobileOperatorID FROM #Results

									 WHERE RowNumber BETWEEN('+convert(varchar(10), @PageIndex)+' -1) * '+convert(varchar(10), @PageSize)+' + 1 AND((('+convert(varchar(10), @PageIndex)+' -1) * '+convert(varchar(10), @PageSize)+' + 1) + '+convert(varchar(10), @PageSize)+') - 1
									order by LastAttemptDateTime desc ; DROP TABLE #Results'
		 End

		 Else if (@OnlyCount=1) 
		 Begin 
		 set @Query=@Query+' Select COUNT(*) as Count FROM #Results; DROP TABLE #Results'
		 End

		 
 exec (@Query) 

                     
                      
                      

END