CREATE PROCEDURE sp_GetEmployeesWorking
@date DATETIME,
@store INT
AS 
SELECT
Employee.Name,
Shift.StartDateTime,
Shift.EmployeeID
From

Store INNER JOIN [Shift] ON Store.ID = [Shift].StoreID INNER JOIN Employee ON [Shift].EmployeeID = Employee.ID
WHERE
STORE.ID = @store and
(shift.StartDateTime between (@date) and dateadd(day, 1, @date) or
shift.EndDateTime between (@date) and dateadd(day, 1, @date))
