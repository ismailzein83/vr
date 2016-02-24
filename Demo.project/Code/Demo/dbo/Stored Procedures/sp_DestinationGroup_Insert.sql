



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_DestinationGroup_Insert]
	@DestinationType int,
	@GroupSettings	nvarchar(MAX),
	@Id int out
AS
BEGIN
	Insert into dbo.DestinationGroup([DestinationType], [GroupSettings])
	Values(@DestinationType, @GroupSettings)

	Set @Id = @@IDENTITY
	END