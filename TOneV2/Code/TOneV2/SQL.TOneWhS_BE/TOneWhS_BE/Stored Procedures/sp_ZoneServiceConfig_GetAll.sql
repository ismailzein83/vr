
create procedure TOneWhs_BE.sp_ZoneServiceConfig_GetAll
as
Begin
   SET NOCOUNT ON;

	Select  conf.ServiceFlag, conf.Name, conf.Settings from ZoneServiceConfig conf With(NOLOCK);

End