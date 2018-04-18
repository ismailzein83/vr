CREATE PROCEDURE [runtime].[sp_RuntimeNodeState_TryUpdateHeartBeat] 
	@RuntimeNodeID uniqueidentifier,
	@InstanceID uniqueidentifier,
	@CPUUsage decimal(6, 2),
	@AvailableRAM decimal(24, 4),
	@DiskInfos nvarchar(max),
	@NbOfEnabledProcesses int,
	@NbOfRunningProcesses int
AS
BEGIN
	
	UPDATE runtime.RuntimeNodeState
	SET LastHeartBeatTime = GETDATE(),
		CPUUsage = @CPUUsage,
		AvailableRAM = @AvailableRAM,
		DiskInfos = @DiskInfos,
		NbOfEnabledProcesses = @NbOfEnabledProcesses,
		NbOfRunningProcesses = @NbOfRunningProcesses
	WHERE RuntimeNodeID = @RuntimeNodeID AND InstanceID = @InstanceID

END