DROP TABLE #CodesWithValidGroups
GO
 with codeAllCGs as
 (SELECT code.[ID] CodeId
      ,code.[Code]
	  ,code.CodeGroupID ExistingGroupId
	  ,existingGroup.Code ExistingGroupCode
      ,grp.ID ValidGroupId
	  ,grp.Code ValidGroupCode
	  , ROW_NUMBER() OVER(PARTITION BY code.ID ORDER BY grp.Code desc)  codeGroupPriority
  FROM [TOneWhS_BE].[SupplierCode] code
  JOIN [TOneWhS_BE].[CodeGroup] existingGroup ON code.CodeGroupID = existingGroup.ID
  JOIN [TOneWhS_BE].[CodeGroup] grp ON code.Code like grp.Code + '%'
  
)

select * 
INTO #CodesWithValidGroups
from codeAllCGs
where codeGroupPriority = 1 AND ExistingGroupId <> ValidGroupId
order by CodeId, ValidGroupCode desc

select * from #CodesWithValidGroups

UPDATE code
SET CodeGroupID = codesWithValidGroups.ValidGroupId
FROM [TOneWhS_BE].[SupplierCode] code
JOIN #CodesWithValidGroups CodesWithValidGroups ON code.ID = codesWithValidGroups.CodeId

