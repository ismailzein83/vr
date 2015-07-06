CREATE procedure [sec].[sp_View_GetByType]
@Type int
as
begin
select v.Name PageName,m.Name ModuleName from sec.[View] v INNER JOIN sec.[Module] m ON v.Module=m.Id where v.[Type]=@Type
end