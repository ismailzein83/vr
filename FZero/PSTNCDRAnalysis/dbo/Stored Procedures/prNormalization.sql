



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================


CREATE PROCEDURE [dbo].[prNormalization]
	
AS


	UPDATE CDR SET  switch = s.Name FROM  SwitchProfiles s JOIN CDR c ON s.Id=c.SourceID 
	UPDATE CDR SET  in_type = s.in_type FROM  AN_In_Trunk_Type s JOIN CDR c ON s.SwitchId=c.SourceID and s.In_Trunk=c.In_Trunk 
	UPDATE CDR SET  out_type = s.out_type FROM  AN_Out_Trunk_Type s JOIN CDR c ON s.SwitchId=c.SourceID and s.Out_Trunk=c.OUT_TRUNK 


  update CDR 
  set b_temp=isnull(prefixToAdd,'') + isnull(substring(a.CdPN,SubstringStartIndex,SubstringLength),a.cdpn) ,ignore=b.Ignore 
  from CDR a inner join NormalizationRules b  on  a.CdPN like  b.Prefix+'%' and len(a.CdPN) = b.callLength  and   isnull(a.out_TRUNK,'NULL') = b.Out_TrunckName 
  where b_temp is null 
  
  
  update CDR
  set a_temp=isnull(prefixToAdd,'') + isnull(substring(a.CgPN,SubstringStartIndex,SubstringLength),a.CgPN)
  from  CDR a inner join CDRAnalysis.dbo.NormalizationRules b on  a.CgPN like  b.Prefix+'%' and   len(a.CgPN) = b.callLength and  in_TRUNK = b.in_TrunckName 
  where a_temp is null 
  
  update CDR set IsNormalized=1 where IsNormalized=0 
  

ALTER INDEX ALL ON CDR
REBUILD WITH (FILLFACTOR = 80, SORT_IN_TEMPDB = ON,
              STATISTICS_NORECOMPUTE = ON);