public class Case {
    public int Id { get; set; }
    public string Type { get; set; }

    public CaseTypeA CaseTypeA { get; set; }
    public CaseTypeB CaseTypeB { get; set; }
    public CaseTypeC CaseTypeC { get; set; }
}

public interface IAuditable {
    int Id { get; set; }
}

public class CaseTypeA : IAuditable {
    public int Id { get; set; }
    public int CaseId { get; set; }
    public string ExtraA { get; set; }
}

public class CaseTypeB : IAuditable {
    public int Id { get; set; }
    public int CaseId { get; set; }
    public string ExtraB { get; set; }
}

public class CaseTypeC : IAuditable {
    public int Id { get; set; }
    public int CaseId { get; set; }
    public string ExtraC { get; set; }
}

public class CaseAudit {
    public int Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public ActionType Action { get; set; }
    public DateTime Timestamp { get; set; }
}

public enum ActionType {
    Created,
    Updated
}
