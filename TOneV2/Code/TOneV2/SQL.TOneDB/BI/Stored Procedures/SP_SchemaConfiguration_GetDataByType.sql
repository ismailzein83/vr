CREATE procedure [BI].[SP_SchemaConfiguration_GetDataByType]
@Type int
as
begin
select sc.ID,sc.Name,sc.[Type],sc.Configuration from SchemaConfiguration sc where sc.[Type]=@Type;
end