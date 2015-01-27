CREATE TABLE [dbo].[BilateralAlertingMessage] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [AlertID]     INT            NOT NULL,
    [AgreementID] INT            NOT NULL,
    [ZoneID]      INT            NULL,
    [ActionDate]  DATETIME       CONSTRAINT [DF_BilateralAlertingMessage_ActionDate] DEFAULT (getdate()) NULL,
    [UserID]      INT            NULL,
    [Message]     NVARCHAR (MAX) NULL
);

