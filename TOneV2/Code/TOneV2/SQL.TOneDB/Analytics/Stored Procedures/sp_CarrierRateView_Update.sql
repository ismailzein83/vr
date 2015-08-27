-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [Analytics].[sp_CarrierRateView_Update]
@RateID BIGINT,
@ServiceIDsSummation INT
AS
BEGIN
UPDATE Rate
SET ServicesFlag = @ServiceIDsSummation
WHERE RateID = @RateID
END