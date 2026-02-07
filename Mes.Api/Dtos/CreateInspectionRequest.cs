public record CreateInspectionRequest(
    int ExecutionId,
    string Result,
    string Status
);

public class PassInspectionRequest
{
    public int ExecutionId { get; set; }
    public int Quantity { get; set; }
}

public class FailInspectionRequest
{
    public int ExecutionId { get; set; }
    public List<DefectItem> Defects { get; set; } = [];
}

public class DefectItem
{
    public int DefectId { get; set; }
    public int Quantity { get; set; }
}
