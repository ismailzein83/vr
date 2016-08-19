-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_RuntimeManager_TryUpdateHeartBeat] 
	@InstanceID uniqueidentifier,
	@ServiceURL varchar(255),
	@TimeoutInSeconds int
AS
BEGIN
	IF NOT EXISTS (SELECT TOP 1 NULL FROM runtime.RuntimeManager with (nolock) WHERE ID = 1)
	BEGIN
		INSERT INTO runtime.RuntimeManager
		(ID, InstanceID, ServiceURL, LastHeartBeatTime)
		SELECT 1, @InstanceID, @ServiceURL, GETDATE()
		WHERE NOT EXISTS (SELECT TOP 1 NULL FROM runtime.RuntimeManager WHERE ID = 1)
	END

	DECLARE @IsUpdated bit
    UPDATE runtime.RuntimeManager
    SET InstanceID = @InstanceID,
		ServiceURL = @ServiceURL,
		LastHeartBeatTime = GETDATE(),
		@IsUpdated = 1
	WHERE ID = 1
	AND (InstanceID = @InstanceID OR DATEDIFF(ss, [LastHeartBeatTime], GETDATE()) > @TimeoutInSeconds)
	
	SELECT ISNULL(@IsUpdated, 0)
END