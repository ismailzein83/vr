CREATE PROCEDURE [common].[sp_VRTimeZone_GetAll]
as
Begin
    SET NOCOUNT ON;
	Select tz.ID,tz.Name, tz.Settings from [common].[VRTimeZone] tz With(NOLOCK);
End