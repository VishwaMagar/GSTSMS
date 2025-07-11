USE [GSTHSMS]
GO
/****** Object:  StoredProcedure [dbo].[sp_SMS]    Script Date: 11-07-2025 23:00:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sp_SMS]
    @flag NVARCHAR(50),
	  @WingName NVARCHAR(50) = NULL,
    @MemberIds NVARCHAR(MAX) = NULL,
    @Email NVARCHAR(100)=NULL,
    @Password NVARCHAR(100)=NULL,



	@MaintananceTypeId int =null,
	@EntityCode nvarchar(50)=null,
@MemberCode nvarchar(50)=null,





@complainId int = null,
@complaintsubTypeid int = null,
@complaintcode nvarchar(50) = null,
@Discription nvarchar(360) = null,
@complaintdate datetime = null,
@RaisedBy nvarchar(50) = null,
@Statusid int =  null,
@ResolvedDate datetime = null,
@BankCode NVARCHAR(50) = NULL,
@AllCode NVARCHAR(50) = NULL,
@AccountTypeId INT = NULL,
@OpeningBalance DECIMAL(18,2) = NULL,
@IFSCCode NVARCHAR(50) = NULL,
@UPIId NVARCHAR(50) = NULL,
@AccountNo NVARCHAR(20) = NULL,
@AddedDate DATETIME = NULL,
@ISActive BIT = 1,
@BankId int=null,
@SocietyName nvarchar(350)=null,
@NewCode VARCHAR(20) =null,





    @NoticeCode VARCHAR(20) = NULL,
    @NoticeTitle NVARCHAR(100) = NULL,
    @Description NVARCHAR(MAX) = NULL,
    @PublishDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @SendBy VARCHAR(20) = NULL,
    @CreatedDate DATETIME = NULL,
    @WingId INT = NULL,
  
    @Document NVARCHAR(350) = NULL,

    
    @ExpenseCode NVARCHAR(50) = NULL,
    @PaymentId NVARCHAR(100) = NULL,
    @PaymentModeId INT = NULL,

    @VendorName NVARCHAR(350)=NULL,
	@VendorType int = NULL,
    @ServiceSubTypeId INT=NULL,
    @PhoneNumber NVARCHAR(16)=NULL,
    @AlternatePhoneno NVARCHAR(16)=NULL,
    @Address NVARCHAR(270)=NULL,

    -- Document 1
    @SubTypeId1 INT=NULL,
    @Document1 NVARCHAR(100)=NULL,

    -- Document 2
    @SubTypeId2 INT=NULL,
    @Document2 NVARCHAR(100)=NULL,

    @Date DATETIME = NULL,

    -- Bank Info
   


	@PaymentTo NVARCHAR(50)=NULL,
	@ExpenseTypeId NVARCHAR(50)=NULL,
	
	@ExpenseName NVARCHAR(100)=NULL,
	@GSTTypeId INT=NULL,
	@Amount DECIMAL(18,2)=NULL,
	
	@AddedBy NVARCHAR(50)=NULL,


	
	@EBudgetCode NVARCHAR(50) = null,
	@EventCode NVARCHAR(50) = null,
	@ActualCost DECIMAL(10,2) = null,
	@BudgetAddedDate DATETIME =null,
	@AllocatedBudget DECIMAL(10,2) = null,
	@BudgetStatus INT = NULL,
	@EventName nvarchar(350) = NULL,
	@EBudgetId INT = NULL,
	@PaymentMode INT = NULL,
    @TransactionId_ChequeId NVARCHAR(100) = NULL,




    @BankName_Code NVARCHAR(20) = NULL,
    @TransactionCode NVARCHAR(20) = NULL,	
@WorkerCode NVARCHAR(20) = NULL,
@AttendanceMonth NVARCHAR(7) = NULL,
    @PaymentBy NVARCHAR(20) = NULL,
    @PaidTo NVARCHAR(20) = NULL,
    @PaymentPurpose NVARCHAR(100) = NULL,
    @PaidDate DATETIME = NULL,
	     @TransactionType INT = NULL,
 




	@Month INT=NULL,
    @Year INT=NULL,

	--vraj prop
	@EventDate DATETIME = NULL,



	@SelectedMonth INT = null,


	
	@MaintenanceCode nvarchar(50) = null,
	@BankCodeName nvarchar(50)=null,
	
	@ChequeId nvarchar(250)=null,
	@AttachmentPath nvarchar(100)=null,
	@PdfPath nvarchar(100)=null

AS
BEGIN




 ----------------------For Send Email By Vishwaraj Magar ---------------------------------------------------------------------------------------------------------------------------------------------




-- IF (@Flag = 'FetchMemberVM')
--    BEGIN
--        SELECT 
--            m.MemberId,
--            m.FullName,
--			m.PhoneNumber,
--            m.Email,
--            w.WingName,
--            f.FlatCode
--        FROM GSTtblRegistrationMember m
--        INNER JOIN GSTtblFlat f ON m.FlatCode = f.FlatCode
--        INNER JOIN GSTtblFloor fl ON f.FloorId = fl.FloorId
--        INNER JOIN GSTtblWing w ON fl.WingId = w.WingId
--        WHERE 
--            (@WingName IS NULL OR @WingName = 'All' OR w.WingName = @WingName)
--            AND (@MemberIds IS NULL OR m.MemberId IN (
--                SELECT value FROM STRING_SPLIT(@MemberIds, ',')
--            ))
--        ORDER BY 
--            LEFT(f.FlatCode, 1),                                 
--            TRY_CAST(SUBSTRING(f.FlatCode, 2, LEN(f.FlatCode)) AS INT)  
--    END



-- IF (@Flag = 'FetchAccountantVM')
--    BEGIN
--SELECT TOP 1 s.StaffId, s.StaffCode, s.Email, s.FlatCode, s.ContactNumber
--FROM GSTtblStaff s
--INNER JOIN GSTtblRole r ON s.RoleId = r.RoleId
--WHERE r.RoleId = '2' AND s.IsActive = 1

--END






 ----------------------For LOGIN By Accountant ---------------------------------------------------------------------------------------------------------------------------------------------
	IF @flag = 'LoginRK'
BEGIN
    SELECT 
    S.StaffId,
    S.Email,
    S.Password,
    S.RoleId,
    R.RoleName  
FROM 
    GSTtblStaff S
INNER JOIN 
    GSTtblRole R ON S.RoleId = R.RoleId
WHERE 
    S.Email = @Email AND S.Password = @Password AND S.RoleId = 2
END


-----------------------For Count Total Members in Society-----------------------------------------------------------------------------------------------
 IF @Flag = 'CountMembersNK'
    BEGIN
        SELECT COUNT(*) AS TotalMembers FROM GSTtblRegistrationMember;
    END

----------------------Fetch  all member information for show in list---------------------------------------
IF @Flag = 'FetchAllMemberDetailsNK'
BEGIN
   Select FlatCode,FullName,Email,PhoneNumber,PossessionDate,Gender,FamilyMemberCount,NoofVehicle,RegisterationDate from GSTtblRegistrationMember
END














 ---------------------- -----------------------------------------------------------------------   All Merged Queries   ---------------------------------------------------------------------------------------------------------------------------------------------


  ---------------------- -----------------------------------------------------------------------   Dashboard   ---------------------------------------------------------------------------------------------------------------------------------------------
 
 

 --Yash Pawar Dashboard
if(@Flag='TotalComplaints')
begin
  select Count(*) from GSTtblComplaints
end
if(@Flag='PendingComplaints')
begin
  select COUNT(*) from GSTtblComplaints where StatusId=12
end
if(@Flag='SolvedComplaints')
begin
  select COUNT(*) from GSTtblComplaints where StatusId=13
end

if(@Flag='CashinHand')
begin
SELECT 
    ISNULL(SUM(CASE WHEN TransactionType = 26 THEN Amount ELSE 0 END), 0) - ISNULL(SUM(CASE WHEN TransactionType = 27 THEN Amount ELSE 0 END), 0) 
    AS CashInHand
FROM GSTtblTransaction
end


if(@Flag='TotalWing')
begin
  select WingId,WingName from GSTtblWing
  end

if(@FLAG='BankBalance')
begin

SELECT 
    OpeningBalance,
    CASE 
        WHEN IFSCCode LIKE 'HDFC%' THEN 'HDFC'
    WHEN IFSCCode LIKE 'KKBK%' THEN 'KKBK'
        ELSE IFSCCode
    END AS BankShortCode
FROM GSTtblBankDetails
WHERE AllCode = 'SC001';

end

if(@Flag='DueMaintenance')
begin
WITH ScheduledMaintenance AS (
    SELECT ISNULL(SUM(Amount), 0) AS TotalScheduled
    FROM GSTtblMaintenanceDetails
),
PaidMaintenance AS (
    SELECT ISNULL(SUM(Amount), 0) AS TotalPaid
    FROM GSTtblTransaction
    WHERE PaymentPurpose = 'Maintenance' 
)

SELECT 
 
    (sm.TotalScheduled - pm.TotalPaid) AS TotalDue
FROM ScheduledMaintenance sm
CROSS JOIN PaidMaintenance pm;
end

if(@Flag='TotalCreditedAndDebited')
begin


SELECT 
    ISNULL(T.TotalCreditedCash, 0) AS TotalCreditedCash,
    ISNULL(T.TotalDebitedCash, 0) AS TotalDebitedCash,
    ISNULL(B.TotalOpeningBalance, 0) AS TotalOpeningBalance
FROM 
(
    SELECT   
        SUM(CASE WHEN TransactionType = 26 THEN Amount ELSE 0 END) AS TotalCreditedCash,
        SUM(CASE WHEN TransactionType = 27 THEN Amount ELSE 0 END) AS TotalDebitedCash
    FROM GSTtblTransaction
    WHERE MONTH(PaidDate) = @Month
) AS T
CROSS JOIN 
(
    SELECT SUM(OpeningBalance) AS TotalOpeningBalance
    FROM GSTtblBankDetails
    WHERE AllCode LIKE 'SC001%'
) AS B;





end

if(@Flag='TotalBalance')
begin
  SELECT SUM(OpeningBalance) AS TotalOpeningBalance
FROM GSTtblBankDetails
WHERE AllCode LIKE 'SC001%';
end

if(@Flag='EventExpens')
begin
  
  SELECT 
    e.EventName,
    SUM(b.AllocatedBudget)   AS TotalAllocatedBudget,
    SUM(b.ActualCost)        AS TotalActualCost
FROM GSTtblEvent e
JOIN GSTtblEventBudget b 
    ON e.EventCode = b.EventCode
WHERE MONTH(e.FromDate) = @Month
GROUP BY e.EventName
ORDER BY e.EventName;
end


if(@Flag='RedListMember')
begin
-- Step 1: Expected maintenance for last 3 full months for ALL registered members
WITH ExpectedMaintenance AS (
    SELECT 
        r.MemberCode,
        r.FullName,
        r.FlatCode,
        LEFT(r.FlatCode, 1) AS WingLetter,
        FORMAT(m.ScheduledDate, 'yyyy-MM') AS MonthYear,
        m.Amount
    FROM GSTtblRegistrationMember r
    CROSS JOIN GSTtblMaintenanceDetails m
    WHERE 
        m.ScheduledDate >= DATEADD(MONTH, -3, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AND
        m.ScheduledDate < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
),

-- Step 2: Get any transaction done by member in that period (regardless of purpose)
PaidAnyTransaction AS (
    SELECT 
        t.PaymentBy AS MemberCode,
        FORMAT(t.PaidDate, 'yyyy-MM') AS MonthYear
    FROM GSTtblTransaction t
    WHERE 
        t.PaidDate >= DATEADD(MONTH, -3, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AND
        t.PaidDate < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
    GROUP BY t.PaymentBy, FORMAT(t.PaidDate, 'yyyy-MM')
),

-- Step 3: Unpaid months = expected maintenance with no matching transaction
UnpaidMonths AS (
    SELECT 
        em.MemberCode,
        em.FullName,
        em.FlatCode,
        em.WingLetter,
        COUNT(*) AS UnpaidMonthCount,
        SUM(em.Amount) AS TotalDue
    FROM ExpectedMaintenance em
    LEFT JOIN PaidAnyTransaction pt
        ON em.MemberCode = pt.MemberCode AND em.MonthYear = pt.MonthYear
    WHERE pt.MonthYear IS NULL
    GROUP BY em.MemberCode, em.FullName, em.FlatCode, em.WingLetter
)







-- Step 4: Final result - Top 5 defaulters
SELECT TOP 5 
    u.FullName AS [FullName],
    'Wing ' + u.WingLetter AS [WingName],
    u.FlatCode AS [FlatNo],
    CONCAT(UnpaidMonthCount, ' Month(s)') AS [MaintenancePending],
    FORMAT(TotalDue, ' #,##0.00') AS [MaintenanceDuePending]
FROM UnpaidMonths u
WHERE UnpaidMonthCount >= 1
ORDER BY TotalDue DESC;
end

if(@Flag='MaintananceStatus')
begin
 
WITH TotalMaintenance AS (
    SELECT ISNULL(SUM(Amount), 0) AS TotalAmount
    FROM GSTtblMaintenanceDetails
),
PaidMaintenance AS (
    SELECT ISNULL(SUM(t.Amount), 0) AS PaidAmount
    FROM GSTtblTransaction t
    WHERE t.PaymentPurpose LIKE '%maintenance%' 
)

-- Final output
SELECT
    tm.TotalAmount,
    pm.PaidAmount,
    (tm.TotalAmount - pm.PaidAmount) AS PendingAmount,
    CAST(CAST(pm.PaidAmount AS FLOAT) * 100 / NULLIF(tm.TotalAmount, 0) AS DECIMAL(5,2)) AS PaidPercentage,
    CAST(CAST((tm.TotalAmount - pm.PaidAmount) AS FLOAT) * 100 / NULLIF(tm.TotalAmount, 0) AS DECIMAL(5,2)) AS UnpaidPercentage
FROM TotalMaintenance tm
CROSS JOIN PaidMaintenance pm;




end



if(@Flag='WorkerPayment')
begin



--DECLARE @Yearrr INT = YEAR(GETDATE()); -- Current Year

WITH AllWorkers AS (
    SELECT WorkerId, WorkerCode, WorkerName
    FROM GSTtblWorkerDetails
    WHERE Status = 17
),

PaidWorkers AS (
    SELECT DISTINCT PaidTo
    FROM GSTtblTransaction
    WHERE 
        MONTH(PaidDate) = @Month
        AND YEAR(PaidDate) = YEAR(GETDATE())
        AND PaidTo LIKE 'WOR%'  -- Identify workers by prefix
)

SELECT 
    (SELECT COUNT(*) 
     FROM AllWorkers 
     WHERE WorkerCode IN (SELECT PaidTo FROM PaidWorkers)
    ) AS Completed,

    (SELECT COUNT(*) 
     FROM AllWorkers 
     WHERE WorkerCode NOT IN (SELECT PaidTo FROM PaidWorkers)
    ) AS Pending




  end


if(@Flag='MaintenanceStatus')
begin
       
DECLARE @Yearr INT = YEAR(GETDATE());

-- Total maintenance billed for this Wing
WITH TotalMaintenance AS (
    SELECT ISNULL(SUM(Amount), 0) AS TotalAmount
    FROM GSTtblMaintenanceDetails
    WHERE Collectfor = @WingId
),

-- Total paid amount for that Wing in selected month/year
PaidMaintenance AS (
    SELECT ISNULL(SUM(t.Amount), 0) AS PaidAmount
    FROM GSTtblTransaction t
    INNER JOIN GSTtblMaintenanceDetails md
        ON t.EntityCode = md.MaintananceCode
    WHERE 
        MONTH(t.PaidDate) = @Month
        AND YEAR(t.PaidDate) = @Yearr
        AND md.Collectfor = @WingId
)

-- Final Output: Summary with safe percentages and no negatives
SELECT
    tm.TotalAmount,
    pm.PaidAmount,

    -- Safe Pending Amount (no negative)
    CASE 
        WHEN (tm.TotalAmount - pm.PaidAmount) < 0 THEN 0 
        ELSE (tm.TotalAmount - pm.PaidAmount) 
    END AS PendingAmount,

    -- Paid Percentage capped at 100
    CASE 
        WHEN tm.TotalAmount = 0 THEN 0
        WHEN (CAST(pm.PaidAmount AS FLOAT) * 100 / tm.TotalAmount) > 100 THEN 100
        ELSE CAST(CAST(pm.PaidAmount AS FLOAT) * 100 / tm.TotalAmount AS DECIMAL(5,2))
    END AS PaidPercentage,

    -- Unpaid Percentage (no negative)
    CASE 
        WHEN tm.TotalAmount = 0 THEN 0
        WHEN (tm.TotalAmount - pm.PaidAmount) < 0 THEN 0
        ELSE CAST(CAST((tm.TotalAmount - pm.PaidAmount) AS FLOAT) * 100 / tm.TotalAmount AS DECIMAL(5,2))
    END AS UnpaidPercentage

FROM TotalMaintenance tm
CROSS JOIN PaidMaintenance pm;
end



 
 ---------------------- -----------------------------------------------------------------------   Society Ac.Details Bank   ---------------------------------------------------------------------------------------------------------------------------------------------
 
 

 -------------------------------------mukesh----------------------------
IF (@flag = 'AddBankSocietyMS')
BEGIN
    DECLARE @LastNo INT
   -- DECLARE @NewCode VARCHAR(20)

    SELECT @LastNo = MAX(CAST(SUBSTRING(BankCode, 5, LEN(BankCode)) AS INT))
    FROM GSTtblBankDetails
    WHERE ISNUMERIC(SUBSTRING(BankCode, 5, LEN(BankCode))) = 1
      AND BankCode LIKE 'BANC%'

    IF @LastNo IS NULL
        SET @LastNo = 0

    SET @NewCode = 'BANC' + RIGHT('000' + CAST(@LastNo + 1 AS VARCHAR), 3)

    INSERT INTO GSTtblBankDetails (
        BankCode, AllCode, AccountTypeId, OpeningBalance, IFSCCode,
        UPIId, AccountNo, AddedDate, ISActive
    )
    VALUES (
        @NewCode, @AllCode, @AccountTypeId, @OpeningBalance,
        @IFSCCode, @UPIId, @AccountNo, ISNULL(@AddedDate, GETDATE()), @ISActive
    )
END


IF (@flag = 'FetchAccountTypeMS')
BEGIN
    SELECT SubTypeId, SubTypeName 
    FROM GSTtblSubTypes
    WHERE TypeId = 5
END

----------------------Fetch All Bank information for society bank  show in list -----------------------------------------------------------
If(@Flag='FetchBankDetailsSS')
begin
SELECT 
    B.BankId,
    E.SocietyName As BankHolderName,
    B.AccountNo As BankAccountNumber,
    B.OpeningBalance As Balance,
    B.IFSCCode,
    B.AddedDate As Date,
	B.BankCode
	
FROM 
    GSTtblBankDetails B
LEFT JOIN 
    GSTtblSocietyRegistration S ON B.AllCode = S.SocietyCode
	inner join GSTtblEnquiry E ON S.EnquiryCode=E.EnquiryCode
WHERE 
    B.AllCode LIKE 'SC%'  AND
    B.ISActive = 1    
    ORDER BY 
    B.AddedDate DESC
  
end

----------------------- Disables a bank account by setting ISActive = 0 for the given BankId-----------------------------------------------------------
IF @Flag = 'DisableBankSS'
BEGIN
    UPDATE GSTtblBankDetails
    SET ISActive = 0
    WHERE BankId = @BankId
END
if(@flag = 'FetchTransactionStatementMS')
begin
SELECT 
    ROW_NUMBER() OVER (ORDER BY t.PaidDate) AS [SrNo],  -- Serial Number

    ISNULL(m.FullName, m2.FullName) AS [SenderReceiver],

    t.PaidDate AS [PaidDate],   -- Raw Date

    t.Amount AS [Amount],       --  Raw Amount

    st.SubTypeName AS [Status]  --  Status (Credited/Debited)

FROM GSTtblTransaction AS t

-- PaidTo: staff info
LEFT JOIN GSTtblStaff s ON s.StaffCode = t.PaidTo
LEFT JOIN GSTtblRole r ON r.RoleId = s.RoleId

-- PaymentBy: member info
LEFT JOIN GSTtblRegistrationMember m ON m.MemberCode = t.PaymentBy

-- PaymentBy is staff
LEFT JOIN GSTtblStaff s2 ON s2.StaffCode = t.PaymentBy
LEFT JOIN GSTtblRegistrationMember m2 ON m2.FlatCode = s2.FlatCode
LEFT JOIN GSTtblRole r2 ON r2.RoleId = s2.RoleId

-- TransactionType = SubTypeId → for Credited/Debited
LEFT JOIN GSTtblSubTypes st ON st.SubTypeId = t.TransactionType

WHERE t.BankName_Code = @BankCode
end
IF (@Flag = 'ComplaintFetch')
BEGIN
    SELECT
        c.ComplaintId,
        s.SubTypeName AS ComplaintName,
        c.Description,
        CONVERT(DATE, c.ComplaintDate) AS ComplaintDate,
        rm.FullName AS RaisedBy,
        CASE c.StatusId
            WHEN 12 THEN 'Pending'
            WHEN 13 THEN 'In Progress'
            WHEN 14 THEN 'Solved'
            ELSE 'Unknown'
        END AS StatusName
    FROM GSTtblComplaints c
    INNER JOIN GSTtblAssignTask at ON c.ComplaintId = at.ComplaintId
    INNER JOIN GSTtblStaff st ON at.AssignedByCode = st.StaffCode
    INNER JOIN GSTtblRegistrationMember rm ON st.FlatCode = rm.FlatCode
    INNER JOIN GSTtblSubTypes s ON c.ComplaintSubTypeId = s.SubTypeId
    WHERE s.TypeId = 1
    ORDER BY c.ComplaintDate DESC
	end
	if(@flag =  'viewfetchcomplaint')
begin 
SELECT 
       ROW_NUMBER() OVER (ORDER BY m.FullName, c.ComplaintDate) AS [Sr.No],
    c.ComplaintId,
    m.FullName AS [Secretary Name],
    s.SubTypeName AS [Complaint Type],
    c.Description AS [Complaint Description],
    c.ComplaintDate,
    st.SubTypeName AS [Status],

    -- Document fields
    d.Document AS DocumentPath,
    ds.SubTypeName AS [Document Type]

FROM 
    GSTtblComplaints c

-- Complaint Type
JOIN GSTtblSubTypes s ON c.ComplaintSubTypeId = s.SubTypeId

-- Task Assignment
JOIN GSTtblAssignTask ta ON ta.ComplaintId = c.ComplaintId

-- Status from SubTypes (TypeId = 4 means Status)
LEFT JOIN GSTtblSubTypes st ON ta.StatusId = st.SubTypeId AND st.TypeId = 4

-- Staff Info who assigned task (Secretary)
JOIN GSTtblStaff sf ON ta.AssignedByCode = sf.StaffCode AND sf.RoleId = 3

-- Secretary Name from Member Table
JOIN GSTtblRegistrationMember m ON sf.FlatCode = m.FlatCode

-- Document join using EntityCode (e.g., CMP005)
LEFT JOIN GSTtblDocument d ON d.EntityCode = CONCAT('CMP', RIGHT('000' + CAST(c.ComplaintId AS VARCHAR(10)), 3))

-- Document SubType Name (like Security, Staff...)
LEFT JOIN GSTtblSubTypes ds ON d.SubTypeId = ds.SubTypeId AND ds.TypeId = 1

WHERE 
    c.ComplaintId = @complainId

END



 
 ---------------------- -----------------------------------------------------------------------   Society Ac.Details Cash   ---------------------------------------------------------------------------------------------------------------------------------------------


 IF (@Flag = 'FetchBankDetails')
    BEGIN
        SELECT * FROM GSTtblBankDetails;
    END
	 IF (@Flag = 'FetchCashTransactions')
    BEGIN
       Select * from GSTtblTransaction
    END
	 IF (@Flag = 'TransactionType')
    BEGIN
        SELECT SubTypeName, SubTypeId FROM GSTtblSubTypes WHERE SubTypeId IN (26, 27);
    END
	 IF (@Flag = 'Fetchmembers')
    BEGIN
        select MemberCode as ReceiverCode, FullName as ReceiverName from GSTtblRegistrationMember 
    END
	 IF (@Flag = 'FetchEventHandlers')
    BEGIN
----        select EventHandlerCode as ReceiverCode,FullName as ReceiverName, EventCode
----from GSTtblEvent
----join GSTtblRegistrationMember on GSTtblRegistrationMember.MemberCode=GSTtblEvent.EventHandlerCode
----where StatusId=15
SELECT 
    e.EventHandlerCode AS ReceiverCode,
    m.FullName AS ReceiverName,
    e.EventCode,
	eb.ActualCost as TotalAmount
FROM GSTtblEvent e
JOIN GSTtblRegistrationMember m ON m.MemberCode = e.EventHandlerCode
JOIN GSTtblEventBudget eb ON e.EventCode = eb.EventCode
WHERE 
    e.StatusId = 21        -- Event is approved
    AND eb.BudgetStatus = 21   -- Budget is also approved
    END
	 IF (@Flag = 'FetchVendor')
    BEGIN
--       select VendorCode as ReceiverCode, VendorName as ReceiverName, ExpenseCode
--from GSTtblVendor 
--join GSTtblExpense on GSTtblVendor.VendorCode=GSTtblExpense.PaymentTo
--where StatusId=11
SELECT 
    v.VendorCode AS ReceiverCode, 
    v.VendorName AS ReceiverName, 
    e.ExpenseCode,
	e.Amount as TotalAmount
FROM GSTtblVendor v
JOIN GSTtblExpense e ON v.VendorCode = e.PaymentTo
WHERE 
    e.StatusId = 11 -- Unpaid
    AND NOT EXISTS (
        SELECT 1 
        FROM GSTtblTransaction t
        WHERE 
            t.EntityCode = e.ExpenseCode
            AND t.TransactionType = 27 -- Expense Payment
    )
ORDER BY v.VendorName

    END
	if(@Flag='FetchWorker')
	begin
	--select WorkerCode as ReceiverCode,WorkerName as ReceiverName from GSTtblWorkerDetails

	SELECT
    st.SubTypeName         AS [Role],
    wd.WorkerName          AS [ReceiverName],
    wd.WorkerCode          AS [ReceiverCode],
    bd.AccountNo,
    wd.WorkerContactNo     AS [Contact],
    CONVERT(date, wd.JoiningDate) AS [Date],
    wd.BaseSalary,
    bd.IFSCCode            AS [IFSC Code],
    bd.UPIId               AS [Worker UPI],
    CONVERT(char(7), GETDATE(), 120) AS [AttendanceMonth],

    AttendanceData.DaysPresent,
    CAST(ROUND(wd.BaseSalary / 30.0, 2) AS decimal(10,2)) AS PerdayPayment,

    CAST(
        ROUND(
            (ROUND(wd.BaseSalary / 30.0, 2) * AttendanceData.DaysPresent)
        , 2) 
    AS decimal(10,2)) AS TotalAmount,

    'Pay' AS [PaymentStatus],

    '' AS [TransactionRef],  -- Placeholder for unpaid rows

    DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1) AS MonthDate

FROM GSTtblWorkerDetails wd
INNER JOIN GSTtblBankDetails bd ON wd.WorkerCode = bd.AllCode
INNER JOIN GSTtblSubTypes st ON wd.SubTypeId = st.SubTypeId
INNER JOIN GSTtblTypes tt ON st.TypeId = tt.TypeId

OUTER APPLY (
    SELECT COUNT(*) AS DaysPresent
    FROM GSTtblVisitor v
    WHERE 
        v.VisitorName = wd.WorkerCode
        AND CONVERT(char(7), v.Date, 120) = CONVERT(char(7), GETDATE(), 120)
) AS AttendanceData

WHERE
    tt.TypeId = 17 -- Worker Type
    AND AttendanceData.DaysPresent > 0
    AND NOT EXISTS (
        SELECT 1 
        FROM GSTtblTransaction t
        WHERE 
            t.PaidTo = wd.WorkerCode
            AND FORMAT(t.PaidDate, 'yyyy-MM') = FORMAT(GETDATE(), 'yyyy-MM')
            AND t.TransactionType = 27
    )

ORDER BY wd.WorkerName;

	
	end
	
	if(@Flag='FetchingMaintenance')
	begin
	SELECT 
    MaintananceCode, 
    WingName, 
    MaintenanceName + ' (Wing ' + WingName + ')' AS MaintenanceDisplayName,
    ScheduledDate, 
    CompletedDate 
FROM GSTtblMaintenanceDetails
JOIN GSTtblWing ON GSTtblWing.wingId = GSTtblMaintenanceDetails.Collectfor
WHERE 
    MONTH(ScheduledDate) = @Month AND YEAR(ScheduledDate) = @Year
	end
	
	if(@Flag='FetchingMembersASperwing')
	begin
	SELECT 
    M.MemberCode,
    M.FullName,
    M.FlatCode,
    M.RegisterationDate
FROM GSTtblRegistrationMember M
JOIN GSTtblMaintenanceDetails MD ON 1 = 1 -- Dummy join to allow filter from maintenance
JOIN GSTtblWing W ON W.WingId = MD.Collectfor
WHERE LEFT(M.FlatCode, 1) = LEFT(W.WingName, 1)
AND MD.MaintananceCode = @MaintenanceCode  -- Replace with actual maintenance code

SELECT SUM(Amount) AS TotalAmount
FROM GSTtblFixMaintenance
WHERE MaintenanceCode =@MaintenanceCode
	end
	if(@Flag='SavingCashChequeTransaction')
	begin
	-- Generate Transaction Code
DECLARE @NextId INT = (SELECT ISNULL(MAX(TransactionId), 0) + 1 FROM GSTtblTransaction);
DECLARE @TransactionCodeSM VARCHAR(10);

IF @PaymentMode = 32
    SET @TransactionCodeSM = 'CAS' + RIGHT('000' + CAST(@NextId AS VARCHAR), 3);
ELSE
    SET @TransactionCodeSM = 'TRX' + RIGHT('000' + CAST(@NextId AS VARCHAR), 3);

-- Insert Query
INSERT INTO GSTtblTransaction (
    BankName_Code,
    TransactionCode,
    EntityCode,
    PaymentBy,
    PaidTo,
    Amount,
    PaymentMode,
    PaymentPurpose,
    TransactionId_ChequeId,
    PaidDate,
    TransactionType
)
VALUES (
    @BankCodeName,
    @TransactionCodeSM,
    @EntityCode,
    @PaymentBy,
    @PaidTo,
    @Amount,
    @PaymentMode,
    @PaymentPurpose,
    @ChequeId,
    CURRENT_TIMESTAMP,
    @TransactionType
);
--INSERT INTO GSTtblDocument  VALUES (@PaymentMode, @TransactionCodeSM, @PdfPath, GETDATE())


 IF (@AttachmentPath IS NOT NULL AND LTRIM(RTRIM(@AttachmentPath)) <> '')
BEGIN
    INSERT INTO GSTtblDocument  VALUES (@PaymentMode, @TransactionCodeSM, @AttachmentPath, GETDATE())
END

SELECT @TransactionCodeSM AS TransactionCode;
	end

	if(@Flag='ReceiptData1')
	begin
	select GSTtblTransaction.*,GSTtblRegistrationMember.FullName
from GSTtblTransaction
join GSTtblRegistrationMember on GSTtblRegistrationMember.MemberCode=GSTtblTransaction.PaymentBy
where TransactionCode=@TransactionCode
	end
	if(@Flag='ReceiptData2')
	begin
	Select * from GSTtblFixMaintenance where MaintenanceCode=@MaintenanceCode
	
	end
	if(@Flag='InsertReceiptPdf')
	begin
	 INSERT INTO GSTtblDocument  VALUES (@PaymentMode, @TransactionCode, @AttachmentPath, GETDATE())
	end

	--Digvijay-----------------------------------------------------
	IF @Flag = 'FetchCashTransactionDD'
    BEGIN
--   
   WITH LatestDocument AS (
    SELECT 
        EntityCode, 
        Document,
        [Date],
        ROW_NUMBER() OVER (PARTITION BY EntityCode ORDER BY [Date] DESC) AS rn
    FROM GSTtblDocument
)
SELECT 
    T.TransactionId,
    ISNULL(T.TransactionCode, '-') AS TransactionCode,
    ISNULL(T.EntityCode, '-') AS EntityCode,

    COALESCE(M.FullName, MS.FullName, V.VendorName, '-') AS PaymentByName,
    COALESCE(M2.FullName, MS2.FullName, V2.VendorName, '-') AS PaidToName,

    T.Amount,
    ISNULL(PM.SubTypeName, '-') AS PaymentModeName,
    ISNULL(T.PaymentPurpose, '-') AS PaymentPurpose,
    ISNULL(T.TransactionId_ChequeId, '-') AS TransactionId_ChequeId,
    T.PaidDate,
    ISNULL(TT.SubTypeName, '-') AS TransactionNature,

    ISNULL(D.Document, '-') AS Document,

    -- ✅ नवीन कॉलम: MaintenanceTypeName
    ISNULL(ST.SubTypeName, '-') as SubTypeName

FROM GSTtblTransaction T

-- Filter joins first for performance
INNER JOIN GSTtblSubTypes TT ON T.TransactionType = TT.SubTypeId
INNER JOIN GSTtblSubTypes PM ON T.PaymentMode = PM.SubTypeId

-- Member, Staff, Vendor joins for PaymentBy
LEFT JOIN GSTtblRegistrationMember M ON M.MemberCode = T.PaymentBy
LEFT JOIN GSTtblStaff SF ON SF.StaffCode = T.PaymentBy
LEFT JOIN GSTtblRegistrationMember MS ON MS.FlatCode = SF.FlatCode
LEFT JOIN GSTtblVendor V ON V.VendorCode = T.PaymentBy

-- Member, Staff, Vendor joins for PaidTo
LEFT JOIN GSTtblRegistrationMember M2 ON M2.MemberCode = T.PaidTo
LEFT JOIN GSTtblStaff SF2 ON SF2.StaffCode = T.PaidTo
LEFT JOIN GSTtblRegistrationMember MS2 ON MS2.FlatCode = SF2.FlatCode
LEFT JOIN GSTtblVendor V2 ON V2.VendorCode = T.PaidTo

-- Latest Document Join using CTE
LEFT JOIN LatestDocument D ON D.EntityCode = T.TransactionCode AND D.rn = 1

-- ✅ नवीन जोडणी: TransactionCode -> MaintananceCode -> MaintananceTypeId -> SubTypeName
LEFT JOIN GSTtblMaintenanceDetails MD ON MD.MaintananceCode = T.EntityCode
LEFT JOIN GSTtblSubTypes ST ON ST.SubTypeId = MD.MaintananceTypeId

WHERE 
    T.TransactionType IN (26, 27)
    AND T.PaymentMode IN (32, 34)

ORDER BY 
    T.PaidDate DESC;

    END
	if(@flag='FetchingWorkerDetails')
	begin
	SELECT
    st.SubTypeName         AS [Role],
    wd.WorkerName          AS [ReceiverName],
    wd.WorkerCode          AS [ReceiverCode],
    bd.AccountNo,
    wd.WorkerContactNo     AS [Contact],
    CONVERT(date, wd.JoiningDate) AS [Date],
    wd.BaseSalary,
    bd.IFSCCode            AS [IFSC Code],
    bd.UPIId               AS [Worker UPI],
    CONVERT(char(7), GETDATE(), 120) AS [AttendanceMonth],

    ISNULL(AttendanceData.DaysPresent, 0) AS DaysPresent,
    CAST(ROUND(wd.BaseSalary / 30.0, 2) AS decimal(10,2)) AS PerdayPayment,
    CAST(ROUND((ISNULL(AttendanceData.DaysPresent, 0) * (wd.BaseSalary / 30.0)), 2) AS decimal(10,2)) AS TotalAmount,

    -- Set PaymentStatus
    CASE 
        WHEN TransactionCheck.TransactionExists = 1 THEN 'Paid'
        ELSE 'Pending'
    END AS PaymentStatus,

    '' AS [TransactionRef],
    DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1) AS MonthDate

FROM GSTtblWorkerDetails wd
INNER JOIN GSTtblBankDetails bd ON wd.WorkerCode = bd.AllCode
INNER JOIN GSTtblSubTypes st ON wd.SubTypeId = st.SubTypeId
INNER JOIN GSTtblTypes tt ON st.TypeId = tt.TypeId

-- Attendance Count
OUTER APPLY (
    SELECT COUNT(*) AS DaysPresent
    FROM GSTtblVisitor v
    WHERE 
        v.VisitorName = wd.WorkerCode
        AND CONVERT(char(7), v.Date, 120) = CONVERT(char(7), GETDATE(), 120)
) AS AttendanceData

-- Check if transaction exists for this month
OUTER APPLY (
    SELECT TOP 1 1 AS TransactionExists
    FROM GSTtblTransaction t
    WHERE 
        t.PaidTo = wd.WorkerCode
        AND t.TransactionType = 27 -- Debit
        AND FORMAT(t.PaidDate, 'yyyy-MM') = FORMAT(GETDATE(), 'yyyy-MM')
) AS TransactionCheck

WHERE
    tt.TypeId = 17 -- Worker Type
    AND wd.WorkerCode = @WorkerCode

ORDER BY wd.WorkerName;


	end

	if(@flag='UpdateExpensepaid')
	begin
	update GSTtblExpense set StatusId=10 where ExpenseCode=@ExpenseCode
	end
	if(@flag='UpdateEventpaid')
	begin
	update GSTtblEventBudget set BudgetStatus=10 where EventCode=@EventCode
	end







 ---------------------- -----------------------------------------------------------------------   Worker Payment   ---------------------------------------------------------------------------------------------------------------------------------------------
 

 --------------------------------------------------Abhi Start Here---------------------------------




--------------------------------------------------Abhi Start Here---------------------------------





IF @flag = 'FetchWorkerInformationAD'
BEGIN
    SELECT
        st.SubTypeName         AS [Role],
        wd.WorkerName,
        wd.WorkerCode,
        bd.AccountNo,
        wd.WorkerContactNo     AS [Contact],
        CONVERT(date, wd.JoiningDate) AS [Date],
        wd.BaseSalary,
        bd.IFSCCode            AS [IFSC Code],
        bd.UPIId               AS [Worker UPI],

        -- ✅ Index-friendly formatting
        CONVERT(char(7), v.Date, 120) AS [AttendanceMonth],

        COUNT(*) AS DaysPresent,

        -- ✅ Round Per Day Payment
        CAST(ROUND(wd.BaseSalary / 30.0, 2) AS decimal(10,2)) AS PerdayPayment,

        -- ✅ Multiply Rounded Per Day * Days
        CAST(
            ROUND((ROUND(wd.BaseSalary / 30.0, 2) * COUNT(*)), 2) 
            AS decimal(10,2)
        ) AS AmountToBePaid,

        -- ✅ Payment Status (using EXISTS, as you wanted)
        CASE 
            WHEN EXISTS (
                SELECT 1 
                FROM GSTtblTransaction t WITH (NOLOCK)
                WHERE t.PaidTo = wd.WorkerCode
                  AND FORMAT(t.PaidDate, 'yyyy-MM') = CONVERT(char(7), v.Date, 120)
                  AND t.TransactionType = 27
            )
            THEN 'Paid'
            ELSE 'Pay'
        END AS [PaymentStatus],

        -- ✅ Transaction Reference if Paid
        ISNULL((
            SELECT TOP 1 t.TransactionId_ChequeId
            FROM GSTtblTransaction t WITH (NOLOCK)
            WHERE t.PaidTo = wd.WorkerCode
              AND FORMAT(t.PaidDate, 'yyyy-MM') = CONVERT(char(7), v.Date, 120)
              AND t.TransactionType = 27
            ORDER BY t.PaidDate DESC
        ), '') AS [TransactionRef],

        MAX(DATEFROMPARTS(YEAR(v.Date), MONTH(v.Date), 1)) AS MonthDate

    FROM GSTtblWorkerDetails wd
    INNER JOIN GSTtblBankDetails bd ON wd.WorkerCode = bd.AllCode
    INNER JOIN GSTtblSubTypes st ON wd.SubTypeId = st.SubTypeId
    INNER JOIN GSTtblTypes tt ON st.TypeId = tt.TypeId
    INNER JOIN GSTtblVisitor v ON wd.WorkerCode = v.VisitorName

    WHERE tt.TypeId = 17
      AND v.VisitorName IN ('WOR001','WOR002','WOR003','WOR005','WOR006','WOR007','WOR008')

    GROUP BY
        st.SubTypeName,
        wd.WorkerName,
        wd.WorkerCode,
        bd.AccountNo,
        wd.WorkerContactNo,
        wd.JoiningDate,
        wd.BaseSalary,
        bd.IFSCCode,
        bd.UPIId,
        CONVERT(char(7), v.Date, 120)

    ORDER BY
        MonthDate DESC, wd.WorkerName
END

-----------------------------------------------------Payment for the worker by the Accountant --------------------------------------------------

IF @flag = 'FetchSingleWorkerPaymentData'
BEGIN
    SELECT 
        st.SubTypeName         AS [Role],
        wd.WorkerName,
        wd.WorkerCode,
        bd.AccountNo,
        wd.WorkerContactNo     AS [Contact],
        CONVERT(date, wd.JoiningDate) AS [Date],
        wd.BaseSalary,
        bd.IFSCCode            AS [IFSC Code],
        bd.UPIId               AS [Worker UPI],
        CONVERT(char(7), v.Date, 120) AS [AttendanceMonth],
        COUNT(*) AS DaysPresent,
        CAST(ROUND(wd.BaseSalary / 30.0, 2) AS decimal(10,2)) AS PerdayPayment,
        CAST(ROUND((ROUND(wd.BaseSalary / 30.0, 2) * COUNT(*)), 2) AS decimal(10,2)) AS AmountToBePaid
    FROM GSTtblWorkerDetails wd
    INNER JOIN GSTtblBankDetails bd ON wd.WorkerCode = bd.AllCode
    INNER JOIN GSTtblSubTypes st ON wd.SubTypeId = st.SubTypeId
    INNER JOIN GSTtblTypes tt ON st.TypeId = tt.TypeId
    INNER JOIN GSTtblVisitor v ON wd.WorkerCode = v.VisitorName
    WHERE 
        tt.TypeId = 17
        AND v.VisitorName = @WorkerCode
        AND CONVERT(char(7), v.Date, 120) = @AttendanceMonth
    GROUP BY
        st.SubTypeName,
        wd.WorkerName,
        wd.WorkerCode,
        bd.AccountNo,
        wd.WorkerContactNo,
        wd.JoiningDate,
        wd.BaseSalary,
        bd.IFSCCode,
        bd.UPIId,
        CONVERT(char(7), v.Date, 120)
END


--EXEC sp_SMS @flag = 'FetchSingleWorkerPaymentData', @WorkerCode = 'Worker001', @AttendanceMonth = 'Jul-2025'



IF @flag = 'TransactionPaymentbyAccountant'
BEGIN
    DECLARE @WorkerBankId INT;
    DECLARE @SocietyBalance DECIMAL(18,2);

    -- Step 0: Get Worker BankId
    SELECT @WorkerBankId = BankId
    FROM GSTtblBankDetails
    WHERE AllCode = @PaidTo;

    IF @WorkerBankId IS NULL
    BEGIN
        RAISERROR('❌ Worker bank account not found for the given code.', 16, 1);
        RETURN;
    END

    -- Step 0.1: Check society balance
    SELECT @SocietyBalance = OpeningBalance
    FROM GSTtblBankDetails
    WHERE BankId = 86;

    IF @SocietyBalance IS NULL
    BEGIN
        RAISERROR('❌ Society account not found.', 16, 1);
        RETURN;
    END

    IF @SocietyBalance < @Amount
    BEGIN
        RAISERROR('❌ Insufficient balance in Society Account. Payment cannot proceed.', 16, 1);
        RETURN;
    END

    -- Step 1: Generate TransactionCode
    IF @PaymentMode IN (33, 35)
    BEGIN
        DECLARE @LastTRX INT;

        SELECT @LastTRX = ISNULL(
            MAX(CAST(SUBSTRING(TransactionCode, 4, LEN(TransactionCode)) AS INT)), 0)
        FROM GSTtblTransaction
        WHERE TransactionCode LIKE 'TRX%';

        SET @TransactionCode = 'TRX' + RIGHT('000' + CAST(@LastTRX + 1 AS VARCHAR), 3);
    END
    ELSE
    BEGIN
        DECLARE @LastCAS INT;

        SELECT @LastCAS = ISNULL(
            MAX(CAST(SUBSTRING(TransactionCode, 4, LEN(TransactionCode)) AS INT)), 0)
        FROM GSTtblTransaction
        WHERE TransactionCode LIKE 'CAS%';

        SET @TransactionCode = 'CAS' + RIGHT('000' + CAST(@LastCAS + 1 AS VARCHAR), 3);
    END

    -- Step 2: Begin Transaction block
    BEGIN TRANSACTION;

    -- Step 3: Deduct from society
    UPDATE GSTtblBankDetails
    SET OpeningBalance = OpeningBalance - @Amount
    WHERE BankId = 86;

    IF @@ERROR <> 0
    BEGIN
        ROLLBACK TRANSACTION;
        RAISERROR('❌ Error deducting from society.', 16, 1);
        RETURN;
    END

    -- Step 4: Credit to worker
    UPDATE GSTtblBankDetails
    SET OpeningBalance = OpeningBalance + @Amount
    WHERE BankId = @WorkerBankId;

    IF @@ERROR <> 0
    BEGIN
        ROLLBACK TRANSACTION;
        RAISERROR('❌ Error crediting to worker.', 16, 1);
        RETURN;
    END

    -- Step 5: Insert into GSTtblTransaction
    INSERT INTO GSTtblTransaction (
        BankName_Code, TransactionCode, EntityCode, PaymentBy,
        PaidTo, Amount, PaymentMode, PaymentPurpose,
        TransactionId_ChequeId, PaidDate, TransactionType
    )
    VALUES (
        @BankName_Code, @TransactionCode, @EntityCode, @PaymentBy,
        @PaidTo, @Amount, @PaymentMode, @PaymentPurpose,
        @TransactionId_ChequeId, GETDATE(), @TransactionType
    );

    IF @@ERROR <> 0
    BEGIN
        ROLLBACK TRANSACTION;
        RAISERROR('❌ Transaction insert failed.', 16, 1);
        RETURN;
    END

    COMMIT TRANSACTION;
    PRINT '✅ Payment + Fund transfer successful!';
END






--///////payment checking if its done then only view
-- SPAccountant procedure मधे हे flag case टाक
IF @flag = 'CheckPaymentExists'
BEGIN
    SELECT COUNT(*) AS PaymentCount
    FROM GSTtblTransaction
    WHERE PaidTo = @PaidTo
      AND FORMAT(PaidDate, 'yyyy-MM') = FORMAT(@PaidDate, 'yyyy-MM')
END


IF @flag = 'FetchWorkerPaymentWithTxn'
BEGIN
    SELECT
        st.SubTypeName          AS [Role],
        wd.WorkerName,
        wd.WorkerCode,
        bd.AccountNo,
        wd.WorkerContactNo      AS [Contact],
        CONVERT(date, wd.JoiningDate) AS [Date],
        wd.BaseSalary,
        bd.IFSCCode             AS [IFSC Code],
        bd.UPIId                AS [Worker UPI],
        FORMAT(v.Date, 'yyyy-MM') AS [AttendanceMonth],
        COUNT(*)                AS [DaysPresent],
        CAST(wd.BaseSalary / 30.0 AS decimal(10,2))              AS [PerdayPayment],
        CAST((wd.BaseSalary / 30.0) * COUNT(*) AS decimal(10,2)) AS [AmountToBePaid],
        MAX(tx.TransactionId_ChequeId)                           AS [TransactionRef],
        MAX(tx.PaidDate)                                         AS [PaidDate],         -- ✅ Optional
        pm.SubTypeName                                           AS [PaymentModeName]
    FROM GSTtblWorkerDetails wd
    INNER JOIN GSTtblBankDetails bd ON wd.WorkerCode = bd.AllCode
    INNER JOIN GSTtblSubTypes st    ON wd.SubTypeId = st.SubTypeId
    INNER JOIN GSTtblTypes tt       ON st.TypeId = tt.TypeId
    INNER JOIN GSTtblVisitor v      ON wd.WorkerCode = v.VisitorName
    LEFT JOIN GSTtblTransaction tx  ON tx.PaidTo = wd.WorkerCode
        AND FORMAT(tx.PaidDate, 'yyyy-MM') = FORMAT(v.Date, 'yyyy-MM')
    LEFT JOIN GSTtblSubTypes pm     ON tx.PaymentMode = pm.SubTypeId
    WHERE tt.TypeId = 17
      AND wd.WorkerCode = @WorkerCode
      AND FORMAT(v.Date, 'yyyy-MM') = @AttendanceMonth
    GROUP BY
        st.SubTypeName,
        wd.WorkerName,
        wd.WorkerCode,
        bd.AccountNo,
        wd.WorkerContactNo,
        wd.JoiningDate,
        wd.BaseSalary,
        bd.IFSCCode,
        bd.UPIId,
        FORMAT(v.Date, 'yyyy-MM'),
        pm.SubTypeName
    ORDER BY
        wd.WorkerName,
        FORMAT(v.Date, 'yyyy-MM') DESC
END

--------------------checking society balance for validation -----------------------------------------------------------------------------------------

IF @flag = 'GetSocietyBalance'
BEGIN
    SELECT OpeningBalance FROM GSTtblBankDetails WHERE BankId = 86
END
 
 ---------------------- -----------------------------------------------------------------------   Maintaince Management   ---------------------------------------------------------------------------------------------------------------------------------------------


 ----------------------Sneha   Yadav---------------------------------------
----------------------Fetch  all member maintenance list---------------------------------------
IF (@flag = 'FetchMemberListSY')
BEGIN
   SELECT 
    ROW_NUMBER() OVER (ORDER BY r.FullName) AS [Sr No],
    r.MemberCode,
    w.WingName AS [Wing Name],
    s.SubTypeName AS [Flat Type],
    r.FullName AS [Full Name],
    r.Email,
    t.EntityCode,

    -- Maintenance Type ID
    ISNULL(md.MaintananceTypeId, md2.MaintananceTypeId) AS MaintananceTypeId,

    -- Total Amount
    CASE 
        WHEN t.TransactionId IS NOT NULL THEN ISNULL(md.Amount, 0.00)
        ELSE ISNULL(md2.Amount, 0.00)
    END AS [Total Amount],

    -- Paid Amount
    ISNULL(t.Amount, 0.00) AS [Paid Amount],

    -- ✅ Add Paid Date and Paid Time as separate columns
    CONVERT(date, t.PaidDate) AS PaidDate,
  

    -- Status
    CASE 
        WHEN t.TransactionId IS NULL THEN 'Incomplete'
        WHEN e.SubTypeName = 'Credited' THEN 'Completed'
        ELSE e.SubTypeName 
    END AS [Status]

FROM 
    GSTtblRegistrationMember r

INNER JOIN GSTtblFlat f ON r.FlatCode = f.FlatCode
INNER JOIN GSTtblSubTypes s ON f.FlatTypeId = s.SubTypeId
INNER JOIN GSTtblFloor fl ON f.FloorId = fl.FloorId
INNER JOIN GSTtblWing w ON fl.WingId = w.WingId

LEFT JOIN GSTtblTransaction t 
    ON r.MemberCode = t.PaymentBy 
    AND t.EntityCode IS NOT NULL
    AND t.TransactionType = (SELECT SubTypeId FROM GSTtblSubTypes WHERE SubTypeName = 'Credited')

LEFT JOIN GSTtblSubTypes e ON t.TransactionType = e.SubTypeId

LEFT JOIN GSTtblMaintenanceDetails md 
    ON t.EntityCode = md.MaintananceCode

OUTER APPLY (
    SELECT TOP 1 Amount, MaintananceTypeId
    FROM GSTtblMaintenanceDetails
    WHERE 
        Collectfor = w.WingId
    ORDER BY MaintenanceName DESC
) md2

END




	----------------------Show member maintaince details for details modal---------------------------------------
	IF (@flag = 'MemberMaintenanceDetailsSY')
BEGIN
   -- Step 1: Use passed EntityCode (which is actually MaintenanceCode)
-- No need of TOP 1 or ORDER BY t.PaidDate DESC now

SELECT 
    r.FullName,
    s.SubTypeName AS FlatType,
    t.PaidDate,  -- ✅ Exact PaidDate from transaction
    fix.MaintenanceId,
    fix.Amount AS ChargeAmount,
    fix.MaintenanceCode,
    (SELECT SUM(Amount) FROM GSTtblFixMaintenance WHERE MaintenanceCode = fix.MaintenanceCode) AS TotalAmount
FROM GSTtblFixMaintenance fix
INNER JOIN GSTtblTransaction t ON fix.MaintenanceCode = t.EntityCode
INNER JOIN GSTtblRegistrationMember r ON t.PaymentBy = r.MemberCode
INNER JOIN GSTtblFlat f ON r.FlatCode = f.FlatCode
INNER JOIN GSTtblSubTypes s ON f.FlatTypeId = s.SubTypeId
WHERE 
    fix.MaintenanceCode = @EntityCode -- ✅ Use passed EntityCode (from view click)
    AND t.PaymentBy = @MemberCode
    AND t.TransactionType = (SELECT SubTypeId FROM GSTtblSubTypes WHERE SubTypeName = 'Credited')
ORDER BY fix.MaintenanceId;

end


  ---------------------- -----------------------------------------------------------------------  Expence   ---------------------------------------------------------------------------------------------------------------------------------------------



  IF @Flag = 'FetchExpenseList'
BEGIN
    SELECT
        EXP.ExpenseId,
        EXP.ExpenseCode,
        EXP.PaymentTo,
        VND.VendorName,
        EXP.ExpenseTypeId,
        SUB1.SubTypeName AS ExpenseTypeName,
        EXP.ExpenseName,
        EXP.WingId,
        WNG.WingName, 
        EXP.AddedBy,
        EXP.Description,
        EXP.AddedDate,
        EXP.Amount,
        EXP.StatusId,
        SUB2.SubTypeName AS StatusName,
        BANK.IFSCCode
    FROM GSTtblExpense AS EXP
    LEFT JOIN GSTtblBankDetails AS BANK ON EXP.PaymentTo = BANK.AllCode
    LEFT JOIN GSTtblVendor AS VND ON EXP.PaymentTo = VND.VendorCode
    LEFT JOIN GSTtblSubTypes AS SUB1 ON EXP.ExpenseTypeId = SUB1.SubTypeId 
    LEFT JOIN GSTtblSubTypes AS SUB2 ON EXP.StatusId = SUB2.SubTypeId
    LEFT JOIN GSTtblWing AS WNG ON EXP.WingId = WNG.WingId 
	ORDER BY EXP.AddedDate DESC;
END




IF @Flag = 'GetExpenseByCodePS'
    BEGIN
        SELECT 
            EXP.ExpenseId,
            EXP.ExpenseCode,
            EXP.PaymentTo,
            EXP.ExpenseTypeId,
            EXP.ExpenseName,
            EXP.WingId,
            EXP.AddedBy,
            EXP.Description,
            EXP.AddedDate,
            EXP.Amount,
            EXP.StatusId,
            BANK.IFSCCode,
            VND.VendorName 
        FROM GSTtblExpense AS EXP
        LEFT JOIN GSTtblBankDetails AS BANK ON EXP.PaymentTo = BANK.AllCode
        LEFT JOIN GSTtblVendor AS VND ON VND.VendorCode = BANK.AllCode
        WHERE EXP.ExpenseCode = @ExpenseCode;
    END



 IF @Flag = 'SaveTransactionPS'
    BEGIN
        DECLARE 
            
            --@TransactionCode NVARCHAR(20),
           
            --@PaymentBy NVARCHAR(50),
            --@PaidTo NVARCHAR(50),
            ----@Amount DECIMAL(18,2),
            --@PaymentPurpose NVARCHAR(100),
            --@TransactionType INT = 27,          
            @VendorBankCode NVARCHAR(50)

        -- Generate new TransactionCode like TRX001
        SELECT @TransactionCode = 
            'TRX' + RIGHT('000' + CAST(ISNULL(MAX(CAST(SUBSTRING(TransactionCode, 4, LEN(TransactionCode)) AS INT)) + 1, 1) AS VARCHAR), 3)
        FROM GSTtblTransaction;

        SELECT 
            @EntityCode = EXP.ExpenseCode,
            @PaymentBy = EXP.AddedBy,
            @PaidTo = EXP.PaymentTo,
            @Amount = EXP.Amount,
            @PaymentPurpose = EXP.ExpenseName,
            @VendorBankCode = BANK.BankCode
        FROM GSTtblExpense AS EXP
        LEFT JOIN GSTtblBankDetails AS BANK ON EXP.PaymentTo = BANK.AllCode
        WHERE EXP.ExpenseCode = @ExpenseCode;

        -- Insert into Transaction Table
        IF @Amount IS NOT NULL
        BEGIN
            INSERT INTO GSTtblTransaction
            (
                BankName_Code,
                TransactionCode,
                EntityCode,
                PaymentBy,
                PaidTo,
                Amount,
                PaymentMode,
                PaymentPurpose,
                TransactionId_ChequeId,
                PaidDate,
                TransactionType
            )
            VALUES
            (
                'BANC086',
                @TransactionCode,
                @EntityCode,
                @PaymentBy,
                @PaidTo,
                @Amount,
                @PaymentModeId,
                @PaymentPurpose,
                @PaymentId,
                GETDATE(),
                '27'
            );
            UPDATE GSTtblExpense
            SET StatusId = 10
            WHERE ExpenseCode = @ExpenseCode;

            UPDATE GSTtblBankDetails
            SET OpeningBalance = OpeningBalance - @Amount
            WHERE BankCode = 'BANC086';

            UPDATE GSTtblBankDetails
            SET OpeningBalance = OpeningBalance + @Amount
            WHERE BankCode = @VendorBankCode;
        END
    END


 IF @Flag = 'GetIFSCByExpenseCodePS'
    BEGIN
        SELECT 
            BANK.IFSCCode,
            BANK.AllCode AS VendorCode,
            BANK.OpeningBalance AS Amount,
            VND.VendorName
        FROM GSTtblExpense AS EXP
        INNER JOIN GSTtblBankDetails AS BANK ON EXP.PaymentTo = BANK.AllCode
        LEFT JOIN GSTtblVendor AS VND ON VND.VendorCode = BANK.AllCode
        WHERE EXP.ExpenseCode = @ExpenseCode;
    END


 IF @Flag = 'GetFullExpenseDetailsPS'
BEGIN
    SELECT
        TRX.TransactionCode,
        ISNULL(TRX.Amount, 0) AS TransactionAmount,
        TRX.PaidDate,
        TRX.PaymentMode,
        SUB.SubTypeName AS PaymentModeName,
        EXP.ExpenseName,
        EXP.Description,
        EXP.AddedDate,
        VND.VendorCode,
        CASE 
            WHEN VND.VendorName IS NOT NULL THEN VND.VendorName
            ELSE EXP.PaymentTo
        END AS VendorName,
        VND.Email,
        VND.PhoneNumber,
        VND.Address,
        DOC.DocumentId,
        DOC.Document
    FROM GSTtblExpense AS EXP
    LEFT JOIN GSTtblTransaction AS TRX ON EXP.ExpenseCode = TRX.EntityCode
    LEFT JOIN GSTtblVendor AS VND ON EXP.PaymentTo = VND.VendorCode
    LEFT JOIN GSTtblSubTypes AS SUB ON TRX.PaymentMode = SUB.SubTypeId
    LEFT JOIN GSTtblDocument AS DOC ON DOC.EntityCode = EXP.ExpenseCode 

    WHERE EXP.ExpenseCode = @ExpenseCode;
END






 IF @Flag = 'InsertVendorDetails'
    BEGIN
        DECLARE @GeneratedVendorCode NVARCHAR(10);
        DECLARE @GeneratedBankCode NVARCHAR(10);

        BEGIN TRY
            BEGIN TRANSACTION;

            SELECT @GeneratedVendorCode = 
                'VEN' + RIGHT('000' + CAST(
                    ISNULL(MAX(CAST(SUBSTRING(VendorCode, 4, LEN(VendorCode)) AS INT)), 0) + 1
                    AS VARCHAR), 3)
            FROM GSTtblVendor
            WHERE VendorCode LIKE 'VEN%';
            SELECT @GeneratedBankCode = 
                'BANC' + RIGHT('000' + CAST(
                    ISNULL(MAX(CAST(SUBSTRING(BankCode, 5, LEN(BankCode)) AS INT)), 0) + 1
                    AS VARCHAR), 3)
            FROM GSTtblBankDetails
            WHERE BankCode LIKE 'BANC%';

            INSERT INTO GSTtblVendor
            (
                VendorCode, VendorName,VendorType,ServiceSubTypeId, Email, PhoneNumber,
                AlternatePhoneno, Address, RegistrationDate, IsActive
            )
            VALUES
            (
                @GeneratedVendorCode, @VendorName, '39',@ServiceSubTypeId, @Email, @PhoneNumber,
                @AlternatePhoneno, @Address, GETDATE(), 1
            );
            INSERT INTO GSTtblDocument
            (SubTypeId, EntityCode, Document, Date)
            VALUES
            (@SubTypeId1, @GeneratedVendorCode, @Document1, ISNULL(@Date, GETDATE()));

            INSERT INTO GSTtblDocument
            (SubTypeId, EntityCode, Document, Date)
            VALUES
            (@SubTypeId2, @GeneratedVendorCode, @Document2, ISNULL(@Date, GETDATE()));

            INSERT INTO GSTtblBankDetails
            (
                BankCode, AllCode, AccountTypeId, AddedDate, OpeningBalance,
                IFSCCode, UPIId, AccountNo, ISActive
            )
            VALUES
            (
                @GeneratedBankCode, @GeneratedVendorCode, '30', GETDATE(), @OpeningBalance,
                @IFSCCode, @UPIId, @AccountNo, 1
            );

            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION;
            THROW;
        END CATCH
    END



	IF @Flag = 'GetServiceSubTypes'
BEGIN
    SELECT SubTypeId, SubTypeName
    FROM GSTtblSubTypes
    WHERE TypeId = 13;
END


----------------------------------------Dnyaneee-------------------------------------------





IF @Flag = 'fetchVendorNameDD'
    BEGIN
	SELECT VendorCode, VendorName FROM GSTtblVendor
	END

    IF @Flag = 'fetchWingNameDD'
    BEGIN
	SELECT wingId, WingName FROM GSTtblWing
	END

	IF @Flag = 'fetchGSTTypeDD'
BEGIN
    SELECT GSTTypeId, GSTTypeName FROM GSTtblGSTTypes
END



IF @Flag = 'insertExpenseDD'
BEGIN
    -- Generate new ExpenseCode
    DECLARE

	
	@NextExpenseCode VARCHAR(20)

    SELECT TOP 1 @NextExpenseCode = ExpenseCode 
    FROM GSTtblExpense 
    ORDER BY ExpenseCode DESC

    IF @NextExpenseCode IS NULL OR @NextExpenseCode = ''
        SET @NextExpenseCode = 'EXP001'
    ELSE
    BEGIN
        DECLARE @NumericPart INT
        SET @NumericPart = CAST(SUBSTRING(@NextExpenseCode, 4, LEN(@NextExpenseCode)) AS INT)
        SET @NumericPart = @NumericPart + 1
        SET @NextExpenseCode = 'EXP' + RIGHT('000' + CAST(@NumericPart AS VARCHAR), 3)

    END

    -- Insert into GSTtblExpense
    INSERT INTO GSTtblExpense (
    ExpenseCode,
    PaymentTo,
    ExpenseTypeId,
    ExpenseName,
    WingId,
    AddedBy,
    Description,
    AddedDate,
    Amount,
	 GSTID,
    StatusId
   
)
VALUES (
    @NextExpenseCode,
    @PaymentTo,
    @ExpenseTypeId,
    @ExpenseName,
    @WingId,
    @AddedBy,
    @Description,
    @AddedDate,
    @Amount,
    @GSTTypeId,
	11


    );

    -- Insert into Document table
    IF @Document IS NOT NULL AND LEN(@Document) > 0
    BEGIN
        INSERT INTO GSTtblDocument (
            SubTypeId,
            EntityCode,
            Document,
            Date
        )
        VALUES (
            @ExpenseTypeId,
            @NextExpenseCode,
            @Document,
            @AddedDate
        );
    END
	SELECT @NextExpenseCode AS ExpenseCode;
END



IF @Flag = 'fetchVendorTypeDD'
    BEGIN
	SELECT SubTypeId, SubTypeName 
	FROM GSTtblSubTypes
    WHERE TypeId = 8;

	END


IF @Flag = 'fetchRegularVendorNameDD'
    BEGIN
	SELECT VendorCode, VendorName FROM GSTtblVendor
    WHERE VendorType = 38;

	END


	IF @Flag = 'fetchOtherVendorNameDD'
    BEGIN
	SELECT VendorCode, VendorName FROM GSTtblVendor
    WHERE VendorType = 39;

	END


   ---------------------- -----------------------------------------------------------------------   Event Management   ---------------------------------------------------------------------------------------------------------------------------------------------

   ----------------------------------------------------------Pradnya Donnnn------------------------------------------------------------

   IF (@Flag = 'FetchApprovedEvent')
 BEGIN

SELECT 
    E.EventCode,
	 E.CreatedDate,
    E.EventName,
    M.FullName AS EventHandlerName,
    M.PhoneNumber
FROM GSTtblEvent E
JOIN GSTtblRegistrationMember M ON E.EventHandlerCode = M.MemberCode
WHERE E.StatusId = 21
ORDER BY E.CreatedDate DESC

END


IF (@Flag = 'FetchOpeningBalance')
BEGIN
	SELECT 
    OpeningBalance 
FROM GSTtblBankDetails
WHERE 
    BankCode = 'BANC087'
    AND AccountTypeId = 29;  -- Saving account

END




 IF (@Flag = 'SaveAllocatedBudget')
    BEGIN
        DECLARE @MaxCode VARCHAR(10);
        DECLARE @NumberPart INT;

        -- Get event code from event name
        SELECT @EventCode = EventCode FROM GSTtblEvent WHERE EventName = @EventName;

        -- Get the current max EBudgetCode
        SELECT @MaxCode = MAX(EBudgetCode) FROM GSTtblEventBudget;

        IF @MaxCode IS NOT NULL
        BEGIN
            SET @NumberPart = CAST(SUBSTRING(@MaxCode, 4, LEN(@MaxCode)) AS INT) + 1;
            SET @NewCode = 'EBC' + RIGHT('000' + CAST(@NumberPart AS VARCHAR), 3);
        END
        ELSE
        BEGIN
            SET @NewCode = 'EBC001';
        END

        INSERT INTO GSTtblEventBudget
            (EBudgetCode, EventCode, ActualCost, BudgetAddedDate, AllocatedBudget, BudgetStatus)
        VALUES
            (@NewCode, @EventCode, @ActualCost, @BudgetAddedDate, @AllocatedBudget, @BudgetStatus);
END


IF (@Flag = 'FetchUpdateEvent')
BEGIN
SELECT 
    E.EventCode,
    E.EventName,
    E.CreatedDate,
    M.FullName AS EventHandlerName,
    M.PhoneNumber,
    B.AllocatedBudget,
    D.AccountNo AS BankAccount,
    D.IFSCCode
FROM GSTtblEvent E
JOIN GSTtblEventBudget B ON E.EventCode = B.EventCode
JOIN GSTtblRegistrationMember M ON E.EventHandlerCode = M.MemberCode
JOIN GSTtblBankDetails D ON M.MemberCode = D.AllCode
WHERE E.EventCode = @EventCode
END

IF (@Flag = 'UpdateActualCost')
BEGIN
    UPDATE GSTtblEventBudget
    SET 
        ActualCost = @ActualCost
    WHERE EventCode = @EventCode
END



IF (@Flag = 'ViewEventDetails')
BEGIN
    SELECT 
        E.EventCode,
        E.EventName,
        E.CreatedDate,
        M.FullName AS EventHandlerName,
        M.PhoneNumber,
        B.AllocatedBudget,
        B.ActualCost,
        D.AccountNo AS BankAccount,
        D.IFSCCode
    FROM GSTtblEvent E
    JOIN GSTtblEventBudget B ON E.EventCode = B.EventCode
    JOIN GSTtblRegistrationMember M ON E.EventHandlerCode = M.MemberCode
    JOIN GSTtblBankDetails D ON M.MemberCode = D.AllCode
    WHERE E.EventCode = @EventCode
END



















------------------------savvvv donnnnnnnn---------------------------


----------------------- Approves a budget request by updating its status and added date in the database.--------------------------------------------------------------------


IF(@Flag = 'UpdateToApprovedSS')
BEGIN
UPDATE GSTtblEventBudget SET BudgetStatus = 21,BudgetAddedDate=GETDATE() WHERE EBudgetId= @EBudgetId
 SELECT @@ROWCOUNT AS RowsAffected;  -- ✅ return number of rows affected
end


-----------------------Fetch All Evetsn information show in list --------------------------------------------------------------------

 IF (@Flag = 'FetchEventsSS')
BEGIN
   SELECT 
    --ROW_NUMBER() OVER (ORDER BY E.CreatedDate DESC) AS SrNo,
    E.EventCode,
	B.EBudgetId,
    E.EventName,
    M.FullName AS EventHandlerName,
    E.CreatedDate,
    B.AllocatedBudget,
    B.ActualCost,
    B.BudgetAddedDate,
	BD.IFSCCode AS IFSCCode,
	B.BudgetStatus AS BudgetStatus,
    S.SubTypeName AS BudgetStatusName  -- Get status name from SubType table
FROM 
    GSTtblEventBudget B
LEFT JOIN 
    GSTtblEvent E ON E.EventCode = B.EventCode
LEFT JOIN 
    GSTtblRegistrationMember M ON E.EventHandlerCode = M.MemberCode
LEFT JOIN 
    GSTtblSubTypes S ON S.SubTypeId = B.BudgetStatus
	LEFT JOIN 
    GSTtblBankDetails BD ON BD.Allcode = E.EventHandlerCode

WHERE 
    S.TypeId = 4  -- Ensure it's from status types
	order by E.CreatedDate DESC ;

END



-----------------------------------------Shubham Vaidya-----------------------------------------------------------------

IF (@Flag = 'InsertTransactionSV')
BEGIN
    --DECLARE @TransactionCode NVARCHAR(20);
    DECLARE @AmountToDeduct DECIMAL(18,2);
    DECLARE @EventHandlerCode NVARCHAR(50);

    -- Generate TransactionCode like TRX001, TRX002
    SELECT @TransactionCode =
        'TRX' + RIGHT('000' + CAST(ISNULL(MAX(CAST(SUBSTRING(TransactionCode, 4, LEN(TransactionCode)) AS INT)) + 1, 1) AS VARCHAR), 3)
    FROM GSTtblTransaction;

    -- Get amount to deduct
    SELECT @AmountToDeduct = B.ActualCost
    FROM GSTtblEventBudget B
    WHERE B.EventCode = @EventCode;

    -- Get EventHandlerCode
    SELECT @EventHandlerCode = E.EventHandlerCode
    FROM GSTtblEvent E
    WHERE E.EventCode = @EventCode;

    -- Insert transaction record
    INSERT INTO GSTtblTransaction
    (
        BankName_Code, TransactionCode, EntityCode, PaymentBy, PaidTo,
        Amount, PaymentMode, PaymentPurpose, TransactionId_ChequeId, PaidDate, TransactionType
    )
    SELECT
        'BANC087',              -- ✅ Hardcoded here
        @TransactionCode,
        E.EventCode,
        'STF002',
        E.EventHandlerCode,
        B.ActualCost,
        @PaymentMode,
        E.EventName,
        @TransactionId_ChequeId,
        GETDATE(),
        '27'
    FROM GSTtblEvent E
    JOIN GSTtblEventBudget B ON E.EventCode = B.EventCode
    WHERE E.EventCode = @EventCode;

    -- ✅ Deduct from SC001 BankDetails instead of SuperAdmin bank
    UPDATE GSTtblBankDetails
    SET OpeningBalance = OpeningBalance - @AmountToDeduct
    WHERE AccountTypeId = 29 AND AllCode = 'SC001';

    -- Add to EventHandler's bank balance
    UPDATE GSTtblBankDetails
    SET OpeningBalance = OpeningBalance + @AmountToDeduct
    WHERE AllCode = @EventHandlerCode;

    -- ✅ Update BudgetStatus to 10
    UPDATE GSTtblEventBudget
    SET BudgetStatus = 10
    WHERE EventCode = @EventCode;
END






    ---------------------- -----------------------------------------------------------------------    Community Send Email   ---------------------------------------------------------------------------------------------------------------------------------------------

	 ---------------------- Vishwaraj Magar ---------------------------------------
	

 IF (@Flag = 'FetchMemberVM')
    BEGIN
        SELECT 
            m.MemberId,
            m.FullName,
			m.PhoneNumber,
            m.Email,
            w.WingName,
            f.FlatCode
        FROM GSTtblRegistrationMember m
        INNER JOIN GSTtblFlat f ON m.FlatCode = f.FlatCode
        INNER JOIN GSTtblFloor fl ON f.FloorId = fl.FloorId
        INNER JOIN GSTtblWing w ON fl.WingId = w.WingId
        WHERE 
            (@WingName IS NULL OR @WingName = 'All' OR w.WingName = @WingName)
            AND (@MemberIds IS NULL OR m.MemberId IN (
                SELECT value FROM STRING_SPLIT(@MemberIds, ',')
            ))
        ORDER BY 
            LEFT(f.FlatCode, 1),                                 
            TRY_CAST(SUBSTRING(f.FlatCode, 2, LEN(f.FlatCode)) AS INT)  
    END



 IF (@Flag = 'FetchAccountantVM')
    BEGIN
SELECT TOP 1 s.StaffId, s.StaffCode, s.Email, s.FlatCode, s.ContactNumber
FROM GSTtblStaff s
INNER JOIN GSTtblRole r ON s.RoleId = r.RoleId
WHERE r.RoleId = '2' AND s.IsActive = 1

END



	    ---------------------- -----------------------------------------------------------------------    Community Complaints & Request  ---------------------------------------------------------------------------------------------------------------------------------------------





		-- Fetch Complaint List with hardcoded Status Text
IF (@Flag = 'ComplaintFetch')
BEGIN
    SELECT
        c.ComplaintId,
        s.SubTypeName AS ComplaintName,
        c.Description,
        CONVERT(DATE, c.ComplaintDate) AS ComplaintDate,
        rm.FullName AS RaisedBy,
        CASE c.StatusId
            WHEN 12 THEN 'Pending'
            WHEN 13 THEN 'In Progress'
            WHEN 14 THEN 'Solved'
            ELSE 'Unknown'
        END AS StatusName
    FROM GSTtblComplaints c
    INNER JOIN GSTtblAssignTask at ON c.ComplaintId = at.ComplaintId
    INNER JOIN GSTtblStaff st ON at.AssignedByCode = st.StaffCode
    INNER JOIN GSTtblRegistrationMember rm ON st.FlatCode = rm.FlatCode
    INNER JOIN GSTtblSubTypes s ON c.ComplaintSubTypeId = s.SubTypeId
    WHERE s.TypeId = 1
    ORDER BY c.ComplaintDate DESC
END

if(@flag =  'viewfetchcomplaint')
begin 
SELECT 
       ROW_NUMBER() OVER (ORDER BY m.FullName, c.ComplaintDate) AS [Sr.No],
    c.ComplaintId,
    m.FullName AS [Secretary Name],
    s.SubTypeName AS [Complaint Type],
    c.Description AS [Complaint Description],
    c.ComplaintDate,
    st.SubTypeName AS [Status],

    -- Document fields
    d.Document AS DocumentPath,
    ds.SubTypeName AS [Document Type]

FROM 
    GSTtblComplaints c

-- Complaint Type
JOIN GSTtblSubTypes s ON c.ComplaintSubTypeId = s.SubTypeId

-- Task Assignment
JOIN GSTtblAssignTask ta ON ta.ComplaintId = c.ComplaintId

-- Status from SubTypes (TypeId = 4 means Status)
LEFT JOIN GSTtblSubTypes st ON ta.StatusId = st.SubTypeId AND st.TypeId = 4

-- Staff Info who assigned task (Secretary)
JOIN GSTtblStaff sf ON ta.AssignedByCode = sf.StaffCode AND sf.RoleId = 3

-- Secretary Name from Member Table
JOIN GSTtblRegistrationMember m ON sf.FlatCode = m.FlatCode

-- Document join using EntityCode (e.g., CMP005)
LEFT JOIN GSTtblDocument d ON d.EntityCode = CONCAT('CMP', RIGHT('000' + CAST(c.ComplaintId AS VARCHAR(10)), 3))

-- Document SubType Name (like Security, Staff...)
LEFT JOIN GSTtblSubTypes ds ON d.SubTypeId = ds.SubTypeId AND ds.TypeId = 1

WHERE 
    c.ComplaintId = @complainId

END

-------------------------------------------------- Send Notice --------------------------------------------------------------------------------

		--------------------------------------------------Ritesh Bhaiyaa--------------------------------------------------------------------------------



IF (@Flag = 'FetchNoticeSS')
BEGIN
    SELECT 
    N.NoticeAnnouncementId,
    N.NoticeAnnouncementCode,
    N.NoticeTitle,
    N.Description,
    N.PublishDate,
    N.EndDate,
    N.SendBy,
    R.RoleName AS SendByRole,
    N.CreatedDate,
    D.Document
FROM [GSTtblNotice/Announcement] N
INNER JOIN GSTtblStaff S ON N.SendBy = S.StaffCode
INNER JOIN GSTtblRole R ON S.RoleId = R.RoleId
LEFT JOIN GSTtblDocument D 
    ON D.EntityCode = N.NoticeAnnouncementCode AND D.SubTypeId = 71
WHERE NoticeAnnouncementCode LIKE 'NC%'  
END




-- 📄 Show Notice List
 IF @Flag = 'ShowData'
BEGIN
    SELECT
        N.NoticeAnnouncementCode,
        N.NoticeTitle,
        N.Description,
        N.PublishDate,
        N.EndDate,
        N.SendBy,
        R.RoleName AS SendByRole,
        N.CreatedDate,
        D.Document
    FROM [GSTtblNotice/Announcement] N
    INNER JOIN GSTtblStaff S ON N.SendBy = S.StaffCode
    INNER JOIN GSTtblRole R ON S.RoleId = R.RoleId
    LEFT JOIN GSTtblDocument D ON D.EntityCode = N.NoticeAnnouncementCode AND D.SubTypeId = 71
    WHERE 
        (@NoticeCode IS NULL OR N.NoticeAnnouncementCode = @NoticeCode)
        AND N.NoticeAnnouncementCode LIKE 'NC%'  -- ✅ Only Notices
    ORDER BY N.CreatedDate DESC  -- ✅ Newest first
END


    -- 💾 Save Notice
    ELSE IF @Flag = 'SaveNotice'
    BEGIN
        
        SELECT @NewCode = ISNULL(MAX(TRY_CAST(SUBSTRING(NoticeAnnouncementCode, 3, LEN(NoticeAnnouncementCode) - 2) AS INT)), 0) + 1
        FROM [GSTtblNotice/Announcement]

        SET @NoticeCode = 'NC' + RIGHT('000' + CAST(@NewCode AS VARCHAR), 3)

        INSERT INTO [GSTtblNotice/Announcement] (
            NoticeAnnouncementCode, NoticeTitle, Description,
            PublishDate, EndDate, SendBy, CreatedDate
        ) VALUES (
            @NoticeCode, @NoticeTitle, @Description,
            @PublishDate, @EndDate, @SendBy, @CreatedDate
        )

        -- 📎 Save Document
        IF @Document IS NOT NULL AND @Document <> ''
        BEGIN
            INSERT INTO GSTtblDocument (SubTypeId, EntityCode, Document, [Date])
            VALUES (71, @NoticeCode, @Document, GETDATE())
        END

       -- 🧾 Save Logs by Wing or Specific Members
IF @WingName IS NOT NULL AND @WingName <> ''
BEGIN
    IF @WingName = 'All'
    BEGIN
        INSERT INTO GSTtblNoticAnnouncementLogs (NoticeCode, MemberCode)
        SELECT @NoticeCode, CAST(m.MemberCode AS NVARCHAR)
        FROM GSTtblRegistrationMember m
        INNER JOIN GSTtblFlat f ON m.FlatCode = f.FlatCode
        INNER JOIN GSTtblFloor fl ON f.FloorId = fl.FloorId
        INNER JOIN GSTtblWing w ON fl.WingId = w.WingId
        WHERE NOT EXISTS (
            SELECT 1 FROM GSTtblNoticAnnouncementLogs 
            WHERE NoticeCode = @NoticeCode AND MemberCode = CAST(m.MemberCode AS NVARCHAR)
        )
    END
    ELSE
    BEGIN
        INSERT INTO GSTtblNoticAnnouncementLogs (NoticeCode, MemberCode)
        SELECT @NoticeCode, CAST(m.MemberCode AS NVARCHAR)
        FROM GSTtblRegistrationMember m
        INNER JOIN GSTtblFlat f ON m.FlatCode = f.FlatCode
        INNER JOIN GSTtblFloor fl ON f.FloorId = fl.FloorId
        INNER JOIN GSTtblWing w ON fl.WingId = w.WingId
        WHERE w.WingName = @WingName
        AND NOT EXISTS (
            SELECT 1 FROM GSTtblNoticAnnouncementLogs 
            WHERE NoticeCode = @NoticeCode AND MemberCode = CAST(m.MemberCode AS NVARCHAR)
        )
    END
END
ELSE IF @MemberIds IS NOT NULL AND LEN(@MemberIds) > 0
BEGIN
    DECLARE @xml XML = '<root><id>' + REPLACE(@MemberIds, ',', '</id><id>') + '</id></root>';

    INSERT INTO GSTtblNoticAnnouncementLogs (NoticeCode, MemberCode)
    SELECT @NoticeCode, T.N.value('.', 'VARCHAR(50)')
    FROM @xml.nodes('/root/id') AS T(N)
    WHERE NOT EXISTS (
        SELECT 1 FROM GSTtblNoticAnnouncementLogs 
        WHERE NoticeCode = @NoticeCode AND MemberCode = T.N.value('.', 'VARCHAR(50)')
    )
END


        -- Return NoticeCode
        SELECT @NoticeCode AS NoticeCode
    END



    -- 📋 Insert Single Log
    ELSE IF @Flag = 'InsertNoticeLog'
    BEGIN
        IF @WingName = 'All'
        BEGIN
            INSERT INTO GSTtblNoticAnnouncementLogs (NoticeCode, MemberCode)
            SELECT @NoticeCode, CAST(m.MemberCode AS NVARCHAR)
            FROM GSTtblRegistrationMember m
            INNER JOIN GSTtblFlat f ON m.FlatCode = f.FlatCode
            INNER JOIN GSTtblFloor fl ON f.FloorId = fl.FloorId
            INNER JOIN GSTtblWing w ON fl.WingId = w.WingId
        END
        ELSE IF EXISTS (SELECT 1 FROM GSTtblWing WHERE WingName = @WingName)
        BEGIN
            INSERT INTO GSTtblNoticAnnouncementLogs (NoticeCode, MemberCode)
            SELECT @NoticeCode, CAST(m.MemberCode AS NVARCHAR)
            FROM GSTtblRegistrationMember m
            INNER JOIN GSTtblFlat f ON m.FlatCode = f.FlatCode
            INNER JOIN GSTtblFloor fl ON f.FloorId = fl.FloorId
            INNER JOIN GSTtblWing w ON fl.WingId = w.WingId
            WHERE w.WingName = @WingName
        END
        ELSE IF @MemberCode IS NOT NULL AND LEN(@MemberCode) > 0
        BEGIN
            INSERT INTO GSTtblNoticAnnouncementLogs (NoticeCode, MemberCode)
            VALUES (@NoticeCode, @MemberCode)
        END
    END

    -- 👨‍👩‍👧‍👦 Fetch Members by Wing
    ELSE IF @Flag = 'FetchMember'
    BEGIN
        SELECT
            m.MemberId,
            m.MemberCode,
            m.FullName,
            m.PhoneNumber,
            m.Email,
            w.WingName,
            f.FlatCode
        FROM GSTtblRegistrationMember m
        INNER JOIN GSTtblFlat f ON m.FlatCode = f.FlatCode
        INNER JOIN GSTtblFloor fl ON f.FloorId = fl.FloorId
        INNER JOIN GSTtblWing w ON fl.WingId = w.WingId
        WHERE (@WingName IS NULL OR @WingName = 'All' OR w.WingName = @WingName)
        ORDER BY LEFT(f.FlatCode, 1), TRY_CAST(SUBSTRING(f.FlatCode, 2, LEN(f.FlatCode)) AS INT)
    END

    -- ✏ Update Notice
    ELSE IF @Flag = 'UpdateNotice'
    BEGIN
        UPDATE [GSTtblNotice/Announcement]
        SET NoticeTitle = @NoticeTitle,
            Description = @Description,
            PublishDate = @PublishDate,
            EndDate = @EndDate
        WHERE NoticeAnnouncementCode = @NoticeCode;

        IF @Document IS NOT NULL AND @Document <> ''
        BEGIN
            IF EXISTS (SELECT 1 FROM GSTtblDocument WHERE EntityCode = @NoticeCode AND SubTypeId = 71)
            BEGIN
                UPDATE GSTtblDocument
                SET Document = @Document, [Date] = GETDATE()
                WHERE EntityCode = @NoticeCode AND SubTypeId = 71
            END
            ELSE
            BEGIN
                INSERT INTO GSTtblDocument (SubTypeId, EntityCode, Document, [Date])
                VALUES (71, @NoticeCode, @Document, GETDATE())
            END
        END
 

 end


		    ---------------------- -----------------------------------------------------------------------    Community Notice/Announcement   ---------------------------------------------------------------------------------------------------------------------------------------------


			    ---------------------- -----------------------------------------------------------------------     Reports  ---------------------------------------------------------------------------------------------------------------------------------------------



				-----------------------For Count Total Members in Society-----------------------------------------------------------------------------------------------
 IF @Flag = 'CountMembersNK'
    BEGIN
        SELECT COUNT(*) AS TotalMembers FROM GSTtblRegistrationMember;
    END

----------------------Fetch  all member information for show in list after click on tab---------------------------------------
IF @Flag = 'FetchAllMemberDetailsNK'
BEGIN
   Select FlatCode,FullName,Email,PhoneNumber,PossessionDate,Gender,FamilyMemberCount,NoofVehicle,RegisterationDate from GSTtblRegistrationMember
END

-----------------------For Count Total Workers in Society-----------------------------------------------------------------------------------------------
 IF @Flag = 'CountWorkerNK'
    BEGIN
        SELECT COUNT(*) AS TotalWorkers FROM GSTtblWorkerDetails;
    END
	----------------------Fetch  all Worker information for show in list after click on tab---------------------------------------
IF @Flag = 'FetchAllWorkerDetailsNK'
BEGIN
SELECT 
     S.SubTypeName,           
    W.WorkerName,
    W.WorkerContactNo,
    W.JoiningDate,
    W.RegisterDate,
        W.Address,
    W.BaseSalary
   FROM GSTtblWorkerDetails W
INNER JOIN GSTtblSubTypes S ON W.SubTypeId = S.SubTypeId
END
-----------------------For Count Total Vendor in Society-----------------------------------------------------------------------------------------------
 IF @Flag = 'CountVendorNK'
    BEGIN
        SELECT COUNT(*) AS TotalVendor FROM GSTtblVendor;
    END
-------------------------Fetch  all Vendor information for show in list after click on tab---------------------------------------
IF @Flag = 'FetchAllVendorDetailsNK'
BEGIN
SELECT 
    V.VendorName,
    SST.SubTypeName,
    V.Email,
    V.PhoneNumber,
    V.Address,
    CONVERT(varchar, V.RegistrationDate, 103) AS RegistrationDate
FROM GSTtblVendor V
LEFT JOIN GSTtblSubTypes SST ON V.ServiceSubTypeId = SST.SubTypeId
ORDER BY V.RegistrationDate DESC;
END
------------Calcualate percentage for  show in piechart of DIRECT and INDIRECT expence-----------------
IF @Flag = 'ShowpiechartDirectIndirectNK'
BEGIN
WITH Total AS (
    SELECT SUM(Amount) AS GrandTotal
    FROM GSTtblExpense
    WHERE MONTH(AddedDate) = @Month AND YEAR(AddedDate) = @Year
),
ExpenseGrouped AS (
    SELECT 
        CASE 
            WHEN ExpenseTypeId = '36' THEN 'Direct Expense'
            WHEN ExpenseTypeId = '37' THEN 'Indirect Expense'
            ELSE 'Other'
        END AS ExpenseType,
        SUM(Amount) AS TotalAmount
    FROM GSTtblExpense
    WHERE MONTH(AddedDate) = @Month AND YEAR(AddedDate) =  @Year
    GROUP BY ExpenseTypeId
)
SELECT 
    eg.ExpenseType,
    eg.TotalAmount,
    CAST((eg.TotalAmount * 100.0) / t.GrandTotal AS DECIMAL(5,2)) AS Percentage
FROM ExpenseGrouped eg
CROSS JOIN Total t
END
------------------ show list when Click on piechart of DIRECT and INDIRECT expence-----------------
IF @Flag = 'ShowListDirectIndirectNK'
BEGIN

-- Fetch details with total
SELECT 
    CASE 
        WHEN E.ExpenseTypeId = 36 THEN 'Direct Expense'
        WHEN E.ExpenseTypeId = 37 THEN 'Indirect Expense'
        ELSE 'Other'
    END AS ExpenseTypeName,

    E.ExpenseName,
    W.WingName,
    V.VendorName AS PaidTo,
    
    E.Description,
    CONVERT(VARCHAR(10), E.AddedDate, 103) AS ExpenseDate,
    E.Amount
FROM 
    GSTtblExpense E
LEFT JOIN 
    GSTtblVendor V ON E.PaymentTo = V.VendorCode
LEFT JOIN 
    GSTtblWing W ON E.WingId = W.WingId
WHERE 
    E.ExpenseTypeId = @ExpenseTypeId
    AND MONTH(E.AddedDate) = @Month
    AND YEAR(E.AddedDate) = @Year

UNION ALL

-- Total
SELECT 
    'Total:' AS ExpenseTypeName,
    '' AS ExpenseName,
    '' AS WingName,
    '' AS PaidTo,
    '' AS Description,
    '' AS ExpenseDate,
    SUM(E.Amount) AS Amount
FROM 
    GSTtblExpense E
WHERE 
    E.ExpenseTypeId = @ExpenseTypeId
    AND MONTH(E.AddedDate) = @Month
    AND YEAR(E.AddedDate) = @Year

ORDER BY 
    ExpenseDate DESC;
END


	----------------------Fetch  all Worker Salary for SAlary graph---------------------------------------

IF @Flag = 'GetWorkerSalaryTotalPerMonth'
BEGIN
    SELECT 
        DATENAME(MONTH, T.PaidDate) AS MonthName,
        MONTH(T.PaidDate) AS MonthNumber,
        SUM(T.Amount) AS TotalAmount
    FROM 
        GSTtblTransaction T
    WHERE 
        T.PaymentPurpose = 'Salary'
    GROUP BY 
        DATENAME(MONTH, T.PaidDate), MONTH(T.PaidDate)
    ORDER BY 
        MonthNumber
END

--for show list when click on worker salary column------------------------------

IF @Flag = 'GetWorkerSalaryListByMonth'
BEGIN
    SELECT 
        W.WorkerName,
        S.SubTypeName,
        W.WorkerContactNo,
        W.Address,
        T.Amount,
        T.PaidDate
    FROM 
        GSTtblTransaction T
    INNER JOIN 
        GSTtblWorkerDetails W ON T.PaidTo = W.WorkerCode
    INNER JOIN 
        GSTtblSubTypes S ON W.SubTypeId = S.SubTypeId 
    WHERE 
        T.PaymentPurpose = 'Salary'
        AND MONTH(T.PaidDate) = @Month 
        AND YEAR(T.PaidDate) = @Year 
    ORDER BY 
        T.PaidDate DESC
END


-------------------------------------------------------vraj flags-------------------------------------------------------------------10-7-2025
IF @flag = 'eventbudgetreport'
BEGIN
    SELECT 
        DATENAME(MONTH, e.FromDate) AS MonthName,
        SUM(ISNULL(b.AllocatedBudget, 0)) AS AllocatedBudget,
        SUM(ISNULL(b.ActualCost, 0)) AS ActualCost
    FROM GSTtblEvent e
    INNER JOIN GSTtblEventBudget b ON e.EventCode = b.EventCode
    WHERE e.StatusId = 15
    GROUP BY 
        DATENAME(MONTH, e.FromDate),
        MONTH(e.FromDate)
    ORDER BY 
        MONTH(e.FromDate);
END

IF @flag = 'eventbudgetbymonth'
BEGIN
  SELECT 
    e.EventName,
    e.FromDate,  -- ✅ REQUIRED by BAL layer
    ISNULL(b.AllocatedBudget, 0) AS AllocatedBudget,
    ISNULL(b.ActualCost, 0) AS ActualCost
  FROM GSTtblEvent e
  LEFT JOIN GSTtblEventBudget b ON e.EventCode = b.EventCode
  WHERE 
    MONTH(e.FromDate) = @Month AND YEAR(e.FromDate) = @Year
  ORDER BY e.FromDate;
END


IF @flag = 'eventbudgetdetails'
BEGIN
SELECT 
    e.EventName,
    e.FromDate,
    ISNULL(b.AllocatedBudget, 0) AS AllocatedBudget,
    ISNULL(b.ActualCost, 0) AS ActualCost,
    ISNULL(b.AllocatedBudget, 0) - ISNULL(b.ActualCost, 0) AS BudgetDifference
FROM GSTtblEvent e
LEFT JOIN GSTtblEventBudget b ON e.EventCode = b.EventCode
ORDER BY e.FromDate;

END


IF @flag = 'incomeexpensechart'
BEGIN
    -- ✅ Ensure WITH starts with semicolon
    ;WITH AllMonths AS (
        SELECT 1 AS MonthNumber, 'January' AS MonthName
        UNION ALL SELECT 2, 'February'
        UNION ALL SELECT 3, 'March'
        UNION ALL SELECT 4, 'April'
        UNION ALL SELECT 5, 'May'
        UNION ALL SELECT 6, 'June'
        UNION ALL SELECT 7, 'July'
        UNION ALL SELECT 8, 'August'
        UNION ALL SELECT 9, 'September'
        UNION ALL SELECT 10, 'October'
        UNION ALL SELECT 11, 'November'
        UNION ALL SELECT 12, 'December'
    ),
    TransactionSummary AS (
        SELECT 
            DATEPART(MONTH, PaidDate) AS MonthNumber,
            SUM(CASE WHEN TransactionType = 26 THEN Amount ELSE 0 END) AS TotalIncome,
            SUM(CASE WHEN TransactionType = 27 THEN Amount ELSE 0 END) AS TotalExpense
        FROM GSTtblTransaction
        WHERE DATEPART(YEAR, PaidDate) = ISNULL(@Year, YEAR(GETDATE()))
        GROUP BY DATEPART(MONTH, PaidDate)
    )
    SELECT 
        am.MonthName,
        am.MonthNumber,
        ISNULL(@Year, YEAR(GETDATE())) AS Year,
        ISNULL(ts.TotalIncome, 0) AS TotalIncome,
        ISNULL(ts.TotalExpense, 0) AS TotalExpense
    FROM AllMonths am
    LEFT JOIN TransactionSummary ts ON am.MonthNumber = ts.MonthNumber
    ORDER BY am.MonthNumber;
END

IF @flag = 'incomeexpensedetails'
BEGIN

SELECT  
    DATENAME(MONTH, DATEFROMPARTS(@Year, @Month, 1)) AS MonthName,
    CASE 
        WHEN TransactionType = 26 THEN 'Income'
        WHEN TransactionType = 27 THEN 'Expense'
        ELSE 'Other'
    END AS TypeLabel,
    Amount,
    PaymentPurpose
FROM GSTtblTransaction
WHERE 
    MONTH(PaidDate) = @Month AND
    YEAR(PaidDate) = @Year AND
    TransactionType IN (26, 27)
ORDER BY PaidDate;


end

end

