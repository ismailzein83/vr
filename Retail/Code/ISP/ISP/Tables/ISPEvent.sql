CREATE TABLE [ISP].[ISPEvent] (
    [StartDate]   DATETIME        NULL,
    [EndDate]     DATETIME        NULL,
    [Username]    NVARCHAR (255)  NULL,
    [IP]          VARCHAR (50)    NULL,
    [Service]     VARCHAR (255)   NULL,
    [MacAddress]  VARCHAR (50)    NULL,
    [Download]    DECIMAL (24, 6) NULL,
    [Upload]      DECIMAL (24, 6) NULL,
    [Package]     VARCHAR (255)   NULL,
    [AccountType] VARCHAR (50)    NULL
);



