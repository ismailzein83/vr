CREATE procedure [Mediation_Generic].[sp_MediationRecord_GetSessionIdByStatus]
@MediationDefinitionId uniqueidentifier,
@Status tinyint,
@LastCommittedId bigint,
@SessionTimeout datetime
as
begin

DECLARE @MediationDefinitionId_local uniqueidentifier,
@Status_Local tinyint,
@LastCommittedId_Local bigint,
@SessionTimeout_Local datetime

SELECT @MediationDefinitionId_local = @MediationDefinitionId,
@Status_Local = @Status,
@LastCommittedId_Local  = @LastCommittedId,
@SessionTimeout_Local = @SessionTimeout

	Select aux.SessionId, Max(aux.IsTimedOut) as IsTimedOut from
	(SELECT		[SessionId], 0 as IsTimedOut
	FROM		[Mediation_Generic].[MediationRecord] WITH(NOLOCK) 
	where		(MediationDefinitionId = @MediationDefinitionId_local and EventStatus = @Status_local and EventId <= @LastCommittedId_Local and SessionId is not null)

	union all 

	select		[sessionid], 1 as IsTimedOut--, sum(case when eventstatus = 2 then 0 else 1 end), max(EventTime)
	FROM		[Mediation_Generic].[MediationRecord] WITH(NOLOCK)
        where		(MediationDefinitionId = @MediationDefinitionId_local and EventId <= @LastCommittedId_Local and SessionId is not null)
	group by	SessionId
	--having	SUM(case when eventstatus=2 then 1 else 0 END) = 0 and max(EventTime) < @SessionTimeout_Local)aux
	having		max(EventTime) < @SessionTimeout_Local)aux
	group by aux.SessionId

end