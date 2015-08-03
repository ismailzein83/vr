CREATE PROCEDURE [dbo].[bp_AddSwitchHistory]
	@SwitchID tinyint,
	@Symbol varchar(10),
	@Name varchar(512),
	@Description text,
	@Configuration ntext,
	@LastCDRImportTag varchar(255),
	@LastImport datetime,
	@LastRouteUpdate datetime,
	@UserID int,
	@Enable_CDR_Import char(1),
	@Enable_Routing char(1),
	@LastAttempt datetime

AS
BEGIN
	INSERT INTO [SwitchHistory]
           ([Date]
           ,[SwitchID]
           ,[Symbol]
           ,[Name]
           ,[Description]
           ,[Configuration]
           ,[LastCDRImportTag]
           ,[LastImport]
           ,[LastRouteUpdate]
           ,[UserID]
           ,[Enable_CDR_Import]
           ,[Enable_Routing]
           ,[LastAttempt])
     VALUES
           (GETDATE(),
           	@SwitchID,
			@Symbol,
			@Name,
			@Description,
			@Configuration,
			@LastCDRImportTag,
			@LastImport,
			@LastRouteUpdate,
			@UserID,
			@Enable_CDR_Import,
			@Enable_Routing,
			@LastAttempt
			)
END