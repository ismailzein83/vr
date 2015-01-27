-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [LCR].[sp_ZoneRate_UpdateFromChangedRates]
	@ratesUpdatedAfter timestamp
AS
BEGIN
	
    UPDATE zr
    SET EndEffectiveDate = r.EndEffectiveDate
    FROM LCR.ZoneRate zr WITH (NOLOCK)
    JOIN Rate r WITH (NOLOCK) ON zr.RateID = r.RateID
    WHERE r.timestamp > @ratesUpdatedAfter
END