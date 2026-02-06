<!-- MASTER DATA -->
Plant
-----
PlantId (PK)  
PlantCode  
PlantName  

Line
----
LineId (PK)  
PlantId (FK)  
LineCode  
LineName

Machine
-------
MachineId (PK)  
LineId (FK)  
MachineCode  
MachineName  

Product
-------
ProductId (PK)  
ProductCode  
ProductName

Routing
-------
RoutingId (PK)  
ProductId (FK)  
RoutingName

RoutingStep
-----------
RoutingStepId (PK)  
RoutingId (FK)  
StepNo  
ProcessName  
MachineId (FK)

<!-- Production Order -->
ProductionOrder
---------------
POId (PK)  
POCode  
ProductId (FK)  
PlanQty  
StartDate  
EndDate  
Status    -- Planned / Released / Running / Completed

ProductionOrderStep
-------------------
POStepId (PK)  
POId (FK)  
RoutingStepId (FK)  
Status

<!-- Production Execution / Tracking (CORE) -->

ProductionExecution
-------------------
ExecutionId (PK)  
POId (FK)  
RoutingStepId (FK)  
MachineId (FK)  
StartTime  
EndTime  
InputQty  
OutputQty  
NGQty

<!-- Quality -->

QualityInspection
-----------------
InspectionId (PK)  
ExecutionId (FK)  
Result      -- Pass / Fail  
InspectionTime

Defect
------
DefectId (PK)  
DefectCode  
DefectName

QualityDefect
-------------
QualityDefectId (PK)  
InspectionId (FK)  
DefectId (FK)  
Quantity

<!-- Equipment / Machine Status -->

MachineStatus
-------------
StatusId (PK)  
MachineId (FK)  
Status      -- Running / Idle / Down  
StartTime  
EndTime

DowntimeReason
--------------
ReasonId (PK)  
ReasonCode  
ReasonName

<!-- User / Role -->

User
----
UserId (PK)  
UserName  
Role        -- Operator / Supervisor / Engineer

