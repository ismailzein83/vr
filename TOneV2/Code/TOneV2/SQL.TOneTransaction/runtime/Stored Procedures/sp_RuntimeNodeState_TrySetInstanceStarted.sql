CREATE PROCEDURE [runtime].[sp_RuntimeNodeState_TrySetInstanceStarted] 
	@RuntimeNodeID uniqueidentifier,
	@InstanceID uniqueidentifier,
	@MachineName nvarchar(1000),
	@OSProcessID int,
	@OSProcessName nvarchar(1000),
	@ServiceURL varchar(255),
	@TimeoutInSeconds float
AS
BEGIN
	IF NOT EXISTS (SELECT TOP 1 NULL FROM runtime.RuntimeNodeState with (nolock) WHERE RuntimeNodeID = @RuntimeNodeID)
	BEGIN
		INSERT INTO runtime.RuntimeNodeState
		([RuntimeNodeID], InstanceID, MachineName, OSProcessID, OSProcessName, [ServiceURL], [StartedTime], [LastHeartBeatTime])
		SELECT @RuntimeNodeID, @InstanceID, @MachineName, @OSProcessID, @OSProcessName, @ServiceURL, GETDATE(), GETDATE()
		WHERE NOT EXISTS (SELECT TOP 1 NULL FROM runtime.RuntimeNodeState WHERE RuntimeNodeID = @RuntimeNodeID)
	END

	DECLARE @IsStarted bit = 0

	UPDATE runtime.RuntimeNodeState
	SET InstanceID = @InstanceID,
		MachineName = @MachineName,
		OSProcessID = @OSProcessID,
		OSProcessName = @OSProcessName,
		ServiceURL = @ServiceURL,
		StartedTime = GETDATE(),
		LastHeartBeatTime = GETDATE(),
		@IsStarted = 1
	WHERE RuntimeNodeID = @RuntimeNodeID 
	AND (InstanceID IS NULL OR InstanceID = @InstanceID OR DATEDIFF(ss, [LastHeartBeatTime], GETDATE()) > @TimeoutInSeconds)

	SELECT @IsStarted
END