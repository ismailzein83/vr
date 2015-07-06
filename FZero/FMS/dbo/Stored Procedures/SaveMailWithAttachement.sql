

-- =============================================
-- Author:		<Walid Mougharbel>
-- Create date: <23 February 2014>
-- Description:	<Sends Emails>
-- =============================================
CREATE PROCEDURE [dbo].[SaveMailWithAttachement] 
    @profile_name varchar(250),
	@recipients varchar(250),
	@subject nvarchar(250),
	@body nvarchar(max), 
	@EmailTemplateId int,
	@copy_recipients nvarchar (750),
	@file_attachments varchar(300)

	
AS
BEGIN
Declare @SenderMail varchar(255), @SenderName varchar(255)
SELECT @SenderMail = value FROM SysParameters Where id = 10
SELECT @SenderName = value FROM SysParameters Where id = 11

select * from Emails
Insert into Emails (SenderName, SenderMail, DestinationEmail, Subject, Body, EmailFormat, EmailTemplateId, CC)
values(@SenderName, @SenderMail, @recipients, @subject, @body, 'HTML', @EmailTemplateId , @copy_recipients )


EXEC	[dbo].[prSendMailWithAttachment]
        @profile_name=@profile_name,
		@recipients = @recipients,
		@subject = @subject,
		@body = @body,
		@file_attachments=@file_attachments, 
		@copy_recipients = @copy_recipients
	

	
END