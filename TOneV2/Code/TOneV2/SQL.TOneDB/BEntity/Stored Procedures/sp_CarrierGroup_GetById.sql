-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_CarrierGroup_GetById]
	 (@Id INT =  NULL)
AS
BEGIN
	SET NOCOUNT ON;

SELECT ID, Name, ParentID
FROM BEntity.CarrierGroup cg
			WHERE 
				cg.ID = @Id

END