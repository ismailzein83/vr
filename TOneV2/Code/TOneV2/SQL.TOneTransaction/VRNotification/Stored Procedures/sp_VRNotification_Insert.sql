CREATE Procedure [VRNotification].[sp_VRNotification_Insert]
	@VRNotificationId uniqueIdentifier,
	@UserId int,
	@TypeId uniqueidentifier,
	@ParentType1 nvarchar(255),
	@ParentType2 nvarchar(255),
	@EventKey nvarchar(255),
	@BPProcessInstanceID bigint,
	@Status tinyint,
	@AlertLevelID uniqueidentifier,
	@Description nvarchar(900),
	@ErrorMessage nvarchar(max),
	@Data nvarchar(max)
AS
BEGIN
	
		insert into [VRNotification].[VRNotification] ([ID] ,[UserID] ,[TypeID],[ParentType1],[ParentType2],[EventKey],[BPProcessInstanceID],[Status], [AlertLevelID],[Description],[ErrorMessage],[Data])
		values(@VRNotificationId, @UserId, @TypeId,@ParentType1,@ParentType2,@EventKey,@BPProcessInstanceID,@Status, @AlertLevelID, @Description, @ErrorMessage, @Data)
	
END