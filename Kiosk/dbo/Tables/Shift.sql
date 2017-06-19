CREATE TABLE [dbo].[Shift] (
    [ID]            INT      IDENTITY (1, 1) NOT NULL,
    [EmployeeID]    INT      NOT NULL,
    [StoreID]       INT      NOT NULL,
    [StartDateTime] DATETIME NOT NULL,
    [EndDateTime]   DATETIME NOT NULL,
    CONSTRAINT [PK_Shift] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Shift_Employee] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([ID]),
    CONSTRAINT [FK_Shift_Store] FOREIGN KEY ([StoreID]) REFERENCES [dbo].[Store] ([ID])
);

