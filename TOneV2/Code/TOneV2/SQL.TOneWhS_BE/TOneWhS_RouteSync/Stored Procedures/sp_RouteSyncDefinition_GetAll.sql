create PROCEDURE [TOneWhS_RouteSync].[sp_RouteSyncDefinition_GetAll]
as
Begin
   SET NOCOUNT ON;

	SELECT	rsd.[ID]
			,rsd.[Name]
			,rsd.[Settings]
  FROM		[TOneWhS_RouteSync].[RouteSyncDefinition] rsd

End