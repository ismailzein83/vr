

CREATE PROCEDURE [dbo].[bp_RT_Full_SetSystemMessages](@msgID varchar(500), @message varchar(1024))
	
AS 
		IF NOT EXISTS (SELECT MessageID FROM SystemMessage WHERE MessageID LIKE @msgID)
			INSERT INTO SystemMessage
			(
				MessageID,
				Description,
				Message,
				Updated
			)
			VALUES
			(
				@msgID,
				@msgID,
				@message,
				getdate()
			)
		ELSE
			UPDATE SystemMessage SET [Message]=@message, Updated=GETDATE() WHERE MessageID=@msgID		
	RETURN