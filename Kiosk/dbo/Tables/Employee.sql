CREATE TABLE [dbo].[Employee] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (100) NOT NULL,
    [HourlyWage]  MONEY          NOT NULL,
    [DateOfBirth] DATETIME       NULL,
    [AddressID]   INT            NOT NULL,
    CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Employee_Address] FOREIGN KEY ([AddressID]) REFERENCES [dbo].[Address] ([ID])
);

