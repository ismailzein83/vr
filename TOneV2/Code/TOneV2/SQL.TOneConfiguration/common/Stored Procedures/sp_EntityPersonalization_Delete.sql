

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [common].[sp_EntityPersonalization_Delete]
	@EntityPersonalizationID bigint
AS
BEGIN
	DELETE FROM [common].[EntityPersonalization] WHERE  Id = @EntityPersonalizationID
END