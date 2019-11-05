﻿CREATE TABLE [VR_Invoice].[InvoiceType] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [DevProjectID]     UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [timestamp]        ROWVERSION       NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_InvoiceType_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_InvoiceType_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_InvoiceType] PRIMARY KEY CLUSTERED ([ID] ASC)
);





