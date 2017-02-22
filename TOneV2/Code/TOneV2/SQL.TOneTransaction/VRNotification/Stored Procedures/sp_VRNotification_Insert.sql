CREATE Procedure [VRNotification].[sp_VRNotification_Insert]
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
	@Data nvarchar(max),
	@ID bigint out
AS
BEGIN
	
		insert into [VRNotification].[VRNotification] ([UserID] ,[TypeID],[ParentType1],[ParentType2],[EventKey],[BPProcessInstanceID],[Status], [AlertLevelID],[Description],[ErrorMessage],[Data])
		values(@UserId, @TypeId,@ParentType1,@ParentType2,@EventKey,@BPProcessInstanceID,@Status, @AlertLevelID, @Description, @ErrorMessage, @Data)
	    SET @ID = SCOPE_IDENTITY()
END