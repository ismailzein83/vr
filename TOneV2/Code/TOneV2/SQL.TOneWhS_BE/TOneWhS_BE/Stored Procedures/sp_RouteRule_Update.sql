-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_RouteRule_Update]
	@Id int,
	@Criteria nvarchar(MAX),
	@TypeConfigID int,
	@RuleSettings nvarchar(MAX),
	@Description nvarchar(MAX),
	@BED datetime,
	@EED datetime,
	@ScheduleSettings nvarchar(MAX)
AS
BEGIN

	update TOneWhS_BE.RouteRule
	Set Criteria = @Criteria,
	    TypeConfigID = @TypeConfigID,
	    RuleSettings = @RuleSettings,
	    [Description] = @Description,
	    BED = @BED,
	    EED = @EED,
	    ScheduleSettings = @ScheduleSettings
	Where ID = @Id
END