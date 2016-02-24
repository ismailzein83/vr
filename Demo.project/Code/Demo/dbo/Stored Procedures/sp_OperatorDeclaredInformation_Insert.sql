﻿
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_OperatorDeclaredInformation_Insert]
	@OperatorID int,
	@FromDate datetime,
	@ToDate datetime,
	@ZoneID bigint,
	@Volume int,
	@AmountType int,
	@Attachment bigint, 
	@Notes varchar(max),
	@Id int out
AS
BEGIN
	Insert into dbo.OperatorDeclaredInformation([OperatorID],[FromDate],[ToDate], [ZoneID], [Volume], [AmountType], [Attachment], [Notes])
	Values(@OperatorID,@FromDate,@ToDate, @ZoneID, @Volume, @AmountType, @Attachment, @Notes)

	Set @Id = @@IDENTITY
	END