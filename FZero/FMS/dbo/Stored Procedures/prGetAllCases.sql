

CREATE PROCEDURE [dbo].[prGetAllCases]
(
  @MobileOperatorID int = NULL,
  @FromAttemptDateTime DATETIME = NULL,
  @ToAttemptDateTime DATETIME = NULL,
  @ClientID int = NULL,
  @IsAdmin bit=NULL
)
AS
BEGIN  

declare @Query varchar(max)



if (@IsAdmin=0) 
		 Begin 

set @Query =         'SELECT distinct   ''Case'' + CONVERT(char(20), GeneratedCalls.ID) AS CaseID, (CASE WHEN Max(GeneratedCalls.OriginationNetwork) IS NULL 
                      THEN ''INTERNATIONAL'' WHEN Max(GeneratedCalls.OriginationNetwork) = '''' THEN ''INTERNATIONAL'' ELSE Max(GeneratedCalls.OriginationNetwork) END) as OriginationNetwork , 
                      (CASE WHEN Carriers.Name IS NULL THEN ''Route '' + RIGHT(''000'' + CONVERT(VARCHAR(4),   case when   GeneratedCalls.Type =''GSM'' then 5 else 201 end   ) , 4) WHEN Carriers.Name = '''' THEN ''Route '' + RIGHT(''000'' + CONVERT(VARCHAR(4),   case when   GeneratedCalls.Type =''GSM'' then 5 else 201 end   ) , 4) ELSE ''Route '' + RIGHT(''000'' + CONVERT(VARCHAR(4), Carriers.ID), 4) END) AS Carrier, Max(GeneratedCalls.AttemptDateTime) as AttemptDateTime,  Max(Statuses.Name) AS StatusName, 
                       Max(GeneratedCalls.b_number) as b_number, (CASE WHEN  Max(dbo.GeneratedCalls.StatusID) = 2 THEN  Max(dbo.RecievedCalls.CLI) ELSE '''' END) AS CLI,  Max(GeneratedCalls.ID) as ID, 
                       Max(Users.FullName) AS SourceName,  Max(GeneratedCalls.MobileOperatorID) as MobileOperatorID
FROM         MobileOperators LEFT OUTER JOIN
                      Users ON MobileOperators.UserID = Users.ID RIGHT OUTER JOIN
                      Carriers INNER JOIN
                      GeneratedCalls ON isnull(Carriers.Name,'''') = isnull(GeneratedCalls.Carrier,'''')  LEFT OUTER JOIN
                      Users AS Users_1 RIGHT OUTER JOIN
                      ApplicationUsers AS ApplicationUsers_1 ON Users_1.ID = ApplicationUsers_1.UserID ON GeneratedCalls.AssignedTo = ApplicationUsers_1.ID ON 
                      MobileOperators.ID = GeneratedCalls.MobileOperatorID LEFT OUTER JOIN
                      Users AS Users_3 RIGHT OUTER JOIN
                      MobileOperators AS MobileOperators_1 RIGHT OUTER JOIN
                      RecievedCalls ON MobileOperators_1.ID = RecievedCalls.MobileOperatorID ON Users_3.ID = MobileOperators_1.UserID ON 
                      GeneratedCalls.ID = RecievedCalls.GeneratedCallID LEFT OUTER JOIN
                      Statuses ON GeneratedCalls.StatusID = Statuses.ID where (1=1) '
                      
                      
    End

	if (@IsAdmin=1) 
		 Begin
		 set @Query =         'SELECT     ''Case'' + CONVERT(char(20), GeneratedCalls.ID) AS CaseID, GeneratedCalls.OriginationNetwork, GeneratedCalls.Carrier, GeneratedCalls.AttemptDateTime, 
                      Statuses.Name AS StatusName, GeneratedCalls.b_number, (CASE WHEN dbo.GeneratedCalls.StatusID = 2 THEN dbo.RecievedCalls.CLI ELSE '''' END) AS CLI, 
                      GeneratedCalls.ID, Users.FullName AS SourceName, GeneratedCalls.MobileOperatorID
						FROM         MobileOperators LEFT OUTER JOIN
											  Users ON MobileOperators.UserID = Users.ID RIGHT OUTER JOIN
											  GeneratedCalls LEFT OUTER JOIN
											  Users AS Users_1 RIGHT OUTER JOIN
											  ApplicationUsers AS ApplicationUsers_1 ON Users_1.ID = ApplicationUsers_1.UserID ON GeneratedCalls.AssignedTo = ApplicationUsers_1.ID ON 
											  MobileOperators.ID = GeneratedCalls.MobileOperatorID LEFT OUTER JOIN
											  Users AS Users_3 RIGHT OUTER JOIN
											  MobileOperators AS MobileOperators_1 RIGHT OUTER JOIN
											  RecievedCalls ON MobileOperators_1.ID = RecievedCalls.MobileOperatorID ON Users_3.ID = MobileOperators_1.UserID ON 
											  GeneratedCalls.ID = RecievedCalls.GeneratedCallID LEFT OUTER JOIN
											  Statuses ON GeneratedCalls.StatusID = Statuses.ID
						WHERE   GeneratedCalls.StatusID >1 and  (1 = 1)  '
		 End
    
    
    
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
			    set @Query=@Query +' Group by ''Case'' + CONVERT(char(20), GeneratedCalls.ID), GeneratedCalls.Type,Carriers.Name, GeneratedCalls.ID,Carriers.ID order by AttemptDateTime desc '

		 End
    

	 if (@IsAdmin=1) 
		 Begin
			set @Query=@Query +' order by GeneratedCalls.AttemptDateTime desc '
		 End
    
    
      exec (@Query)                    
                      
                      

END