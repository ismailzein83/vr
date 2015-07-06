CREATE PROCEDURE [dbo].[prImportfromSwitches]
AS
BEGIN
	SET NOCOUNT ON;
    
	DECLARE @ID int
	DECLARE @DatabaseName varchar(100)
    DECLARE @Reference int

	DECLARE Switch_Cursor CURSOR FOR
	SELECT  s.Id as ID, s.DatabaseName as DatabaseName
	FROM Switch_DatabaseConnections s inner join SwitchProfiles sp on s.Id=sp.Id 
	Where sp.AllowAutoImport=1
    
	OPEN Switch_Cursor;
	FETCH NEXT FROM Switch_Cursor INTO @ID, @DatabaseName  ;

	WHILE @@FETCH_STATUS = 0
	   BEGIN TRY
	   
	   set @Reference=  (select top 1 reference from CDR where SourceID=@ID order by ID desc);
	   print @Reference
	   set @Reference=  CAST( isnull((select top 1 reference from CDR where SourceID=@ID order by ID desc) ,'0' ) as int) ;
	   print @Reference
	
	 
	 
		  exec prImportfromSwitch @ID,@DatabaseName, @Reference
		  FETCH NEXT FROM Switch_Cursor INTO @ID, @DatabaseName  ;
	   END TRY
	   BEGIN CATCH
			SELECT
			 ERROR_NUMBER() AS ErrorNumber
			,ERROR_SEVERITY() AS ErrorSeverity
			,ERROR_STATE() AS ErrorState
			,ERROR_PROCEDURE() AS ErrorProcedure
			,ERROR_LINE() AS ErrorLine
			,ERROR_MESSAGE() AS ErrorMessage;
			
			print @DatabaseName

	   END CATCH; 
	CLOSE Switch_Cursor;
	DEALLOCATE Switch_Cursor;
    
    ALTER INDEX ALL ON CDR
REBUILD WITH (FILLFACTOR = 80, SORT_IN_TEMPDB = ON,
              STATISTICS_NORECOMPUTE = ON);
	    
END