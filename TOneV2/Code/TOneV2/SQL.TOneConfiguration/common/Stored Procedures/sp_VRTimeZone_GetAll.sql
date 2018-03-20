CREATE PROCEDURE [common].[sp_VRTimeZone_GetAll]
as
Begin
    SET NOCOUNT ON;
	Select	tz.ID,tz.Name, tz.Settings, tz.CreatedTime, tz.CreatedBy, tz.LastModifiedBy, tz.LastModifiedTime
	from	[common].[VRTimeZone] tz With(NOLOCK)
	ORDER BY tz.[Name]
End