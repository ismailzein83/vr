
CREATE procedure [Mediation_Generic].[sp_MediationRecord_DeleteByEventIds]
@Ids dbo.IDType READONLY
as
begin

DECLARE 

@Ids_Local dbo.IDType

INSERT INTO @Ids_Local

SELECT * FROM @Ids

	delete sr
	FROM [Mediation_Generic].[MediationRecord] sr WITH (NOLOCK)
   JOIN @Ids_Local i ON  sr.EventID = i.ID

end