CREATE TABLE [logging].[ReceivedRequestLog] (
    [ID]                 BIGINT         IDENTITY (1, 1) NOT NULL,
    [ActionName]         NVARCHAR (50)  NOT NULL,
    [Method]             NVARCHAR (50)  NOT NULL,
    [ModuleName]         NVARCHAR (50)  NOT NULL,
    [ControllerName]     NVARCHAR (50)  NOT NULL,
    [URI]                NVARCHAR (MAX) NOT NULL,
    [Path]               NVARCHAR (MAX) NOT NULL,
    [RequestHeader]      NVARCHAR (MAX) NULL,
    [Arguments]          NVARCHAR (MAX) NULL,
    [RequestBody]        NVARCHAR (MAX) NULL,
    [ResponseHeader]     NVARCHAR (MAX) NULL,
    [ResponseStatusCode] NVARCHAR (10)  NULL,
    [IsSucceded]         BIT            NOT NULL,
    [ResponseBody]       NVARCHAR (MAX) NULL,
    [UserId]             INT            NULL,
    [StartDateTime]      DATETIME       NULL,
    [EndDateTime]        DATETIME       NULL
);

