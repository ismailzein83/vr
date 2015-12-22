

-- =============================================
-- Author:		<Walid Mougharbel>
-- Create date: <23 February 2014>
-- Description:	<Sends Emails>
-- =============================================
CREATE PROCEDURE [dbo].[SaveMail] 
    @profile_name varchar(250),
	@recipients varchar(250),
	@subject nvarchar(250),
	@body nvarchar(max), 
	@copy_recipients nvarchar (250)='',
	@EmailTemplateId int

	
AS
BEGIN
Declare @SenderMail varchar(255), @SenderName varchar(255)
SELECT @SenderMail = value FROM SysParameters Where id = 10
SELECT @SenderName = value FROM SysParameters Where id = 11

select * from Emails
Insert into Emails (SenderName, SenderMail, DestinationEmail, Subject, Body, EmailFormat, EmailTemplateId , CC)
values(@SenderName, @SenderMail, @recipients, @subject, @body, 'HTML', @EmailTemplateId, @copy_recipients )


EXEC	[dbo].[prSendMail]
        @profile_name=@profile_name,
		@recipients = @recipients,
		@subject = @subject,
		@body = @body,
		@copy_recipients = @copy_recipients
	

	
END