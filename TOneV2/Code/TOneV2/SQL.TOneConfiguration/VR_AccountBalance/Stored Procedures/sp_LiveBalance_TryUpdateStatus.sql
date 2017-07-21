﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_TryUpdateStatus] 
	@AccountID varchar(50),
	@AccountTypeID uniqueidentifier,
	@Status int,
	@IsDeleted bit
AS
BEGIN
IF  EXISTS(select 1 from [VR_AccountBalance].LiveBalance where AccountId = @AccountID and AccountTypeID =@AccountTypeID) 
	BEGIN
	UPDATE	[VR_AccountBalance].LiveBalance 
	SET		[Status] =@Status,
			IsDeleted = @IsDeleted
	WHERE	 AccountId = @AccountID and AccountTypeID =@AccountTypeID
	END
END