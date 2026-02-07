CREATE DATABASE MES;
GO

USE MES;
GO

CREATE TABLE Plant (
    PlantId INT IDENTITY PRIMARY KEY,
    PlantCode VARCHAR(50),
    PlantName VARCHAR(50)
);
GO

CREATE TABLE Line (
    LineId INT IDENTITY PRIMARY KEY,
    PlantId INT,
    LineCode VARCHAR(50),
    LineName VARCHAR(50),
    CONSTRAINT FK_Plant_Line
        FOREIGN KEY (PlantId)
        REFERENCES Plant(PlantId)
);
GO

CREATE TABLE Machine (
    MachineId INT IDENTITY PRIMARY KEY,
    LineId INT,
    MachineCode VARCHAR(50),
    MachineName VARCHAR(50),
    CONSTRAINT FK_Line_Machine
        FOREIGN KEY (LineId)
        REFERENCES Line(LineId)
);
GO

CREATE TABLE Product (
    ProductId INT IDENTITY PRIMARY KEY,
    ProductCode VARCHAR(50),
    ProductName VARCHAR(50)
);
GO

CREATE TABLE Routing (
    RoutingId INT IDENTITY PRIMARY KEY,
    ProductId INT,
    RoutingName VARCHAR(50),
    CONSTRAINT FK_Routing_Product
        FOREIGN KEY (ProductId)
        REFERENCES Product(ProductId)
);
GO

CREATE TABLE RoutingStep (
    RoutingStepId INT IDENTITY PRIMARY KEY,
    RoutingId INT,
    StepNo VARCHAR(50),
    ProcessName  VARCHAR(225),
    MachineId INT,
    CONSTRAINT FK_Routing_RoutingStep
        FOREIGN KEY (RoutingId)
        REFERENCES Routing(RoutingId),
    CONSTRAINT FK_Routing_Machine
        FOREIGN KEY (MachineId)
        REFERENCES Machine(MachineId)
);
GO

CREATE TABLE ProductionOrder (
    POId INT IDENTITY PRIMARY KEY,
    POCode VARCHAR(50),
    ProductId INT,
    PlantQty INT,
    StartDate DATETIME DEFAULT GETDATE(),
    EndDate DATETIME,
    Status VARCHAR(50) NOT NULL,
    CONSTRAINT CK_ProductionOrder_Status
        CHECK (Status IN ('Planned', 'Released', 'Running', 'Completed')),
    CONSTRAINT FK_ProductionOrder_Product
        FOREIGN KEY (ProductId)
        REFERENCES Product(ProductId)
);
GO

CREATE TABLE ProductionOrderStep (
    POStepId INT IDENTITY PRIMARY KEY,
    POId INT,
    RoutingStepId INT,
    Status VARCHAR(50),
    CONSTRAINT FK_ProductionOrderStep_ProductionOrder
        FOREIGN KEY (POId)
        REFERENCES ProductionOrder(POId),
    CONSTRAINT FK_ProductionOrderStep_RoutingStep
        FOREIGN KEY (RoutingStepId)
        REFERENCES RoutingStep(RoutingStepId)
);
GO

CREATE TABLE ProductionExecution (
    ExecutionId INT IDENTITY PRIMARY KEY,
    POId INT,
    RoutingStepId INT,
    MachineId INT,
    StartTime DATETIME DEFAULT GETDATE(),
    EndTime DATETIME,
    InputQty  INT,
    OutputQty INT,
    NGQty INT,
    CONSTRAINT FK_ProductionExecution_ProductionOrder
        FOREIGN KEY (POId)
        REFERENCES ProductionOrder(POId),
    CONSTRAINT FK_ProductionExecution_RoutingStep
        FOREIGN KEY (RoutingStepId)
        REFERENCES RoutingStep(RoutingStepId),
    CONSTRAINT FK_ProductionExecution_Machine
        FOREIGN KEY (MachineId)
        REFERENCES Machine(MachineId)
);
GO

CREATE TABLE QualityInspection (
    InspectionId INT IDENTITY PRIMARY KEY,
    ExecutionId INT,
    Result VARCHAR(50),
    Status VARCHAR(50),
    CONSTRAINT CK_QualityInspection_Status
        CHECK (Status IN ('Pass', 'Fail')),
    InspectionTime DATETIME,
    CONSTRAINT FK_QualityInspection_ProductionExecution
        FOREIGN KEY (ExecutionId)
        REFERENCES ProductionExecution(ExecutionId)
);
GO

CREATE TABLE Defect (
    DefectId INT IDENTITY PRIMARY KEY,
    DefectCode VARCHAR(50),
    DefectName VARCHAR(50)
);
GO

CREATE TABLE QualityDefect (
    QualityDefectId INT IDENTITY PRIMARY KEY,
    InspectionId INT,
    DefectId INT,
    Quantity INT,
    CONSTRAINT FK_QualityDefect_QualityInspection
        FOREIGN KEY (InspectionId)
        REFERENCES QualityInspection(InspectionId),
    CONSTRAINT FK_QualityDefect_Defect
        FOREIGN KEY (DefectId)
        REFERENCES Defect(DefectId)
);
GO

CREATE TABLE MachineStatus (
    StatusId INT IDENTITY PRIMARY KEY,
    MachineId INT,
    Status VARCHAR(50) NOT NULL,
    StartTime DATETIME DEFAULT GETDATE(),
    EndTime DATETIME,
    CONSTRAINT CK_MachineStatus_Status
        CHECK (Status IN ('Running', 'Idle', 'Down')),
    CONSTRAINT FK_MachineStatus_Machine
        FOREIGN KEY (MachineId)
        REFERENCES Machine(MachineId)
);
GO

CREATE TABLE DowntimeReason (
    ReasonId INT IDENTITY PRIMARY KEY,
    ReasonCode VARCHAR(50),
    ReasonName VARCHAR(50)
);
GO

CREATE TABLE MachineDowntime (
    DowntimeId INT IDENTITY PRIMARY KEY,
    MachineId INT NOT NULL,
    ReasonId INT NOT NULL,
    StartTime DATETIME NOT NULL DEFAULT GETDATE(),
    EndTime DATETIME,
    CONSTRAINT FK_Downtime_Machine
        FOREIGN KEY (MachineId) REFERENCES Machine(MachineId),
    CONSTRAINT FK_Downtime_Reason
        FOREIGN KEY (ReasonId) REFERENCES DowntimeReason(ReasonId)
);
GO

CREATE TABLE Users (
    UserId INT IDENTITY PRIMARY KEY,
    UserName VARCHAR(50),
    Role VARCHAR(50),
    CONSTRAINT CK_User_Role
        CHECK (Role IN ('Operator', 'Supervisor', 'Engineer'))
);
GO

-- insert

-- INSERT INTO ProductionOrder
-- (POCode, ProductId, PlantQty, Status)
-- VALUES
-- ('PO-2026-001', 1, 1000, 'Planned');

