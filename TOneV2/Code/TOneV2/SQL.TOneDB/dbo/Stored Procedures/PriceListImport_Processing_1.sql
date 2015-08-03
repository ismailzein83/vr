
CREATE PROCEDURE [dbo].[PriceListImport_Processing]
	@TableName varchar(100) = 'CDR_Backup'
AS
BEGIN
	SET NOCOUNT ON	
	declare @Insertcmds varchar(4000) 
    declare @Updatecmds varchar(4000) 
    declare Insertcmds cursor 
    for 
    SELECT  'Insert into Code 
                 Select * from '+TABLE_NAME +'Where ID<=0'
    FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME
                            LIKE 'tmpImportedPricelist_%' 
                            open cmds 
                            while 1=1  begin
                            fetch Insertcmds into @Insertcmds 
                            if @@fetch_status != 0 
                            break 
                            exec(@Insertcmds)
                            end 
                            close Insertcmds 
                            deallocate Insertcmds
    declare Updatecmds cursor 
    for 
    SELECT  'UPDATE Code set Code.BeginEffectiveDate ='+TABLE_NAME +'.BeginEffectiveDate
		   from '+TABLE_NAME +'INNER JOIN'+TABLE_NAME +' ON '+TABLE_NAME +'.Code = Code.Code,'+TABLE_NAME +'.ZoneID=Code.ZoneID
		   where '+TABLE_NAME +'.ID>0'
    FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME
                            LIKE 'tmpImportedPricelist_%' 
                            open cmds 
                            while 1=1  begin
                            fetch Updatecmds into @Updatecmds 
                            if @@fetch_status != 0 
                            break 
                            exec(@Updatecmds)
                            end 
                            close Updatecmds 
                            deallocate Updatecmds	
	
END