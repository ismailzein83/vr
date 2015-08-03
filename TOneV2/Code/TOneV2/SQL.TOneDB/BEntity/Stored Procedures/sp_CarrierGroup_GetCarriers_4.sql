-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [BEntity].[sp_CarrierGroup_GetCarriers] 

(@CarrierGroupId INT =  NULL)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


SELECT     dbo.CarrierAccount.CarrierAccountID, dbo.CarrierProfile.Name
FROM         dbo.CarrierProfile INNER JOIN
                      dbo.CarrierAccount ON dbo.CarrierProfile.ProfileID = dbo.CarrierAccount.ProfileID LEFT OUTER JOIN
                      dbo.CarrierGroup ON dbo.CarrierAccount.CarrierGroupID = dbo.CarrierGroup.CarrierGroupID
                      
                      WHERE     (dbo.CarrierGroup.CarrierGroupID = @CarrierGroupId)
END