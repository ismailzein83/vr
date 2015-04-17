





CREATE VIEW [dbo].[vwRepeatedCase]
AS
SELECT     0  MobileOperatorID  ,  '' CLI, getdate() LastAttemptDateTime, GETDATE() FirstAttemptDateTime, 0 ID