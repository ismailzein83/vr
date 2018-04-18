CREATE PROCEDURE [runtime].[sp_RuntimeManager_TryTakePrimaryNode] 
	@InstanceID uniqueidentifier,
	@TimeoutInSeconds float
AS
BEGIN
	IF NOT EXISTS (SELECT TOP 1 NULL FROM runtime.RuntimeManager with (nolock) WHERE ID = 1)
	BEGIN
		INSERT INTO runtime.RuntimeManager
		(ID, InstanceID, TakenTime)
		SELECT 1, @InstanceID, GETDATE()
		WHERE NOT EXISTS (SELECT TOP 1 NULL FROM runtime.RuntimeManager WHERE ID = 1)
	END

	DECLARE @PrimaryNodeInstanceID uniqueidentifier = (SELECT [InstanceID] FROM runtime.RuntimeManager WHERE ID = 1)

	IF @PrimaryNodeInstanceID <> @InstanceID
	BEGIN
	    UPDATE manager
		SET InstanceID = @InstanceID,
			@PrimaryNodeInstanceID = @InstanceID,
			TakenTime = GETDATE()
		FROM runtime.RuntimeManager manager
		LEFT JOIN [runtime].[RuntimeNodeState] node ON manager.InstanceID = node.InstanceID
		WHERE manager.ID = 1
		AND (node.RuntimeNodeID IS NULL OR DATEDIFF(ss, node.[LastHeartBeatTime], GETDATE()) > @TimeoutInSeconds)
	END
	
	SELECT CASE WHEN @PrimaryNodeInstanceID = @InstanceID THEN 1 ELSE 0 END
END