-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_OperatorDeclaredInfo_GetAll]
AS
BEGIN
	SELECT ID,Settings
	FROM dbo.OperatorDeclaredInfo  with(nolock)
END