



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_DestinationGroup_Update]
	@ID int,
	@DestinationType int,
	@GroupSettings	nvarchar(MAX)
AS
BEGIN


	Update dbo.DestinationGroup
		Set DestinationType = @DestinationType,
		GroupSettings = @GroupSettings

	Where ID = @ID
END