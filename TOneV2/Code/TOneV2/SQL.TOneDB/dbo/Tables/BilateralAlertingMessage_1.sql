CREATE TABLE [dbo].[BilateralAlertingMessage] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [AlertID]     INT            NOT NULL,
    [AgreementID] INT            NOT NULL,
    [ZoneID]      INT            NULL,
    [ActionDate]  DATETIME       CONSTRAINT [DF_BilateralAlertingMessage_ActionDate] DEFAULT (getdate()) NULL,
    [UserID]      INT            NULL,
    [Message]     NVARCHAR (MAX) NULL,
    [Type]        VARCHAR (4)    NULL,
    CONSTRAINT [PK_BilateralAlertingMessage] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_AlertID]
    ON [dbo].[BilateralAlertingMessage]([AlertID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AgreementID]
    ON [dbo].[BilateralAlertingMessage]([AgreementID] ASC);

