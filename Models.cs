public class Case {
    public int Id { get; set; }
    public string Type { get; set; }

    public CaseTypeA CaseTypeA { get; set; }
    public CaseTypeB CaseTypeB { get; set; }
    public CaseTypeC CaseTypeC { get; set; }

    public virtual ICollection<CaseAudit> CaseAudit { get; set; } 
}

public interface ICaseAuditable {
    int Id { get; set; }
    int CaseId { get; set; } 
}

public class CaseTypeA : ICaseAuditable {
    public int Id { get; set; }
    public int CaseId { get; set; }
    public string ExtraA { get; set; }
    public string Status { get; set; }
    public int SomeIntValue { get; set; }
    public virtual Case Case { get; set; }
}

public class CaseTypeB : ICaseAuditable {
    public int Id { get; set; }
    public int CaseId { get; set; }
    public string ExtraB { get; set; }


    public virtual Case Case { get; set; } 
}

public class CaseTypeC : ICaseAuditable {
    public int Id { get; set; }
    public int CaseId { get; set; }
    public string ExtraC { get; set; }

    public virtual Case Case { get; set; } 
}

public class CaseAudit {
    public int Id { get; set; }
    public int CaseId { get; set; }
    public ActionType Action { get; set; }
    public DateTime Timestamp { get; set; }

    public string FieldName { get; set; }
    public string FieldValue { get; set; }

    public int UserId { get; set; }
    public string Fullname { get; set; }

    public virtual Case Case { get; set; } 
}

public enum ActionType {
    Created,
    Updated, 

}
