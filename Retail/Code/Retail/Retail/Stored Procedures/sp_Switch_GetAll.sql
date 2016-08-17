-- =============================================
-- Author:		<Mostafa Jawhari>
-- Create date: <06/16/2016>
-- Description:	<sp to get all swtiches>
-- =============================================
CREATE PROCEDURE [Retail].[sp_Switch_GetAll]
AS
BEGIN
	SELECT ID, Name,Settings, CreatedTime
	FROM [Retail_BE].[Switch]  with(nolock)
END