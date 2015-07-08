-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Switch_GetName]
	-- Add the parameters for the stored procedure here
	@SwitchId INT
AS
BEGIN
	SELECT S.Name FROM Switch S WITH (NOLOCK)
WHERE S.SwitchID = @SwitchId
END