
CREATE PROCEDURE [common].sp_VRPop3ReadMessageId_GetLastMessageSendTime
	-- Add the parameters for the stored procedure here
	@ConnectionID uniqueIdentifier,
	@SenderIdentifier varchar(255)
AS
BEGIN
	select MAX(MessageTime) from [common].VRPop3ReadMessageId where ConnectionID=@ConnectionID and SenderIdentifier=@SenderIdentifier
END