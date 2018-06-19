
CREATE PROCEDURE [common].[sp_VRPop3ReadMessageId_GetMessageIdsFromSpecificTime]
	-- Add the parameters for the stored procedure here
	@ConnectionID uniqueIdentifier,
	@SenderIdentifier varchar(255),
	@FromTime datetime
AS
BEGIN
SET NOCOUNT ON;
	select MessageId 
	from [common].VRPop3ReadMessageId 
	where ConnectionID=@ConnectionID and SenderIdentifier=@SenderIdentifier and MessageTime>@FromTime
END