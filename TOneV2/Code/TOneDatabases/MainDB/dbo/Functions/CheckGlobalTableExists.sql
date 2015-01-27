create function [dbo].[CheckGlobalTableExists]
(
@TableName varchar(100)
)
RETURNS int

as
begin
IF NOT EXISTS (SELECT 'x'
          FROM tempdb..sysobjects
          WHERE type = 'U' and NAME = @TableName)
          begin
    return 0
end
return 1
    end