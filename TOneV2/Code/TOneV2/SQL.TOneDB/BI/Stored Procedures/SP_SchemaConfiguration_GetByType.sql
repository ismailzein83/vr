CREATE procedure [BI].[SP_SchemaConfiguration_GetByType]
@Type int
as
begin
select sc.ID,sc.DisplayName,sc.Name,sc.[Type],sc.Configuration from SchemaConfiguration sc where sc.[Type]=@Type;
end