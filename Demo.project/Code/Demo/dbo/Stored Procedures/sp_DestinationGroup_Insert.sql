



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_DestinationGroup_Insert]
	@DestinationType int,
	@GroupSettings	nvarchar(MAX),
	@Name varchar(50),
	@Id int out
AS
BEGIN
	Insert into dbo.DestinationGroup([DestinationType], [GroupSettings],[Name])
	Values(@DestinationType, @GroupSettings,@Name)

	Set @Id = @@IDENTITY
	END