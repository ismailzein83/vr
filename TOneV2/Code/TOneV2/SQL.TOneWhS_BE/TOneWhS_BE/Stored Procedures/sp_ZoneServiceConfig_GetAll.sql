CREATE PROCEDURE [TOneWhS_BE].[sp_ZoneServiceConfig_GetAll]
as
Begin
   SET NOCOUNT ON;

	Select conf.ID, conf.Symbol, conf.Settings, conf.SourceID from ZoneServiceConfig conf With(NOLOCK);

End