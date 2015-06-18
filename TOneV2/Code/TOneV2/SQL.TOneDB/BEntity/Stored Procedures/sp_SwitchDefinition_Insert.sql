


CREATE procedure [BEntity].[sp_SwitchDefinition_Insert]
(	
@Name nvarchar(50)=null,
@Symbol nvarchar(10)=null,
@Description  nvarchar(50)=null,
@EnableCDRImport bit=0,
@EnableRouting bit=0,
@LastAttempt Datetime=null,
@LastImport Datetime =null,
@switchId int OUTPUT
)
as
BEGIN
select * from Switch
insert into  switch(Name,Symbol,Description,Enable_CDR_Import,Enable_Routing,LastAttempt,LastImport) Values(@Name,@Symbol,@Description,@EnableCDRImport,@EnableRouting,@LastAttempt,@LastImport)
set @switchId= scope_Identity()
END