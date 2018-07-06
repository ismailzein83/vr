CREATE TABLE [ICX_Data].[UserSessionData] (
    [UserSession] VARCHAR (110) NOT NULL,
    [StartDate]   DATETIME      NOT NULL
);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_UserSessionData_UserSession]
    ON [ICX_Data].[UserSessionData]([UserSession] ASC);

