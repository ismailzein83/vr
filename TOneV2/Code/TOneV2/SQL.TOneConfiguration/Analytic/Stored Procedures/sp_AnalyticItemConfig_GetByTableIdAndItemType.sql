

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Analytic].[sp_AnalyticItemConfig_GetByTableIdAndItemType]
@TableId INT,
@TtemType INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	aic.[ID],aic.[TableId],aic.[ItemType],aic.[Name],aic.[Title],aic.[Config]
    FROM	[Analytic].[AnalyticItemConfig] aic WITH(NOLOCK) 
    WHERE	TableId = @TableId AND ItemType = @TtemType
END