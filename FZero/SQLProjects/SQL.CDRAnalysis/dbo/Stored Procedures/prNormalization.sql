





-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================


CREATE PROCEDURE [dbo].[prNormalization]
	
AS


	UPDATE CDR SET  switch = s.Name FROM  SwitchProfiles s JOIN CDR c ON s.Id=c.SourceID 
	


  update CDR 
  set Destination=isnull(prefixToAdd,'') + isnull(substring(a.Destination,SubstringStartIndex,SubstringLength),a.Destination) ,ignore=b.Ignore 
  from CDR a inner join NormalizationRules b  on  a.Destination like  b.Prefix+'%' and len(a.Destination) = b.callLength   
  where Destination is null 
  
  
  update CDR
  set MSISDN=isnull(prefixToAdd,'') + isnull(substring(a.MSISDN,SubstringStartIndex,SubstringLength),a.MSISDN)
  from  CDR a inner join .NormalizationRules b on  a.MSISDN like  b.Prefix+'%' and   len(a.MSISDN) = b.callLength
  where MSISDN is null 
  
  update CDR set IsNormalized=1 where IsNormalized is null  
  

ALTER INDEX ALL ON CDR
REBUILD WITH (FILLFACTOR = 80, SORT_IN_TEMPDB = ON,
              STATISTICS_NORECOMPUTE = ON);