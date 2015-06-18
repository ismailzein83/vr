CREATE procedure [BEntity].[sp_SwitchDefinition_Update]
(
	@switchId int=0,
	@Name nvarchar(50)=null,
	@Symbol nvarchar(10)=null,
	@Description  nvarchar(50)=null,
	@EnableCDRImport bit=0,
	@EnableRouting bit=0,
	@LastAttempt Datetime=null,
	@LastImport Datetime =null
)
as
BEGIN

update switch set  Name=isnull(@Name, Name),  Symbol=isnull(@Symbol, Symbol),  Description=isnull(@Description, Description),  Enable_CDR_Import=isnull(@EnableCDRImport, Enable_CDR_Import),  Enable_Routing=isnull(@EnableRouting, Enable_Routing)
,LastAttempt=isnull(@LastAttempt, LastAttempt),
LastImport=isnull(@LastImport, LastImport)
where  SwitchID=@switchId

END