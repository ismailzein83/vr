-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPDefinitionState_GetByKey]
	@DefinitionID int,
	@ObjectKey varchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	ObjectValue
	FROM	bp.BPDefinitionState WITH(NOLOCK) 
	WHERE	DefinitionID = @DefinitionID AND ObjectKey = @ObjectKey
END