
CREATE PROCEDURE [Analytic].[sp_VRReportGeneration_GetAll]

AS
BEGIN
	SELECT ID,Name,[Description],Settings,AccessLevel,CreatedBy,CreatedTime,LastModifiedTime,LastModifiedBy FROM Analytic.VRReportGeneration;
END