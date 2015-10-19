-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_RouteRule_Insert]
	@Criteria nvarchar(MAX),
	@TypeConfigID int,
	@RuleSettings nvarchar(MAX),
	@Description nvarchar(MAX),
	@BED datetime,
	@EED datetime,
	@ScheduleSettings nvarchar(MAX),
	@Id int out
AS
BEGIN

	Insert into TOneWhS_BE.RouteRule ([Criteria], [TypeConfigID], [RuleSettings], [Description], [BED], [EED], [ScheduleSettings])
	Values(@Criteria, @TypeConfigID, @RuleSettings, @Description, @BED, @EED, @ScheduleSettings)
	
	Set @Id = @@IDENTITY
END