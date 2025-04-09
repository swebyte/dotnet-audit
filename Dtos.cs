using System.Text.Json.Serialization;

[JsonDerivedType(typeof(CaseDtoA), "A")]
[JsonDerivedType(typeof(CaseDtoB), "B")]
[JsonDerivedType(typeof(CaseDtoC), "C")]
public interface ICaseDto {
    int Id { get; set; }
    string Type { get; }
}

public class CaseDtoA : ICaseDto {
    public int Id { get; set; }
    public string Type => "A";
    public string ExtraA { get; set; } = string.Empty;
}

public class CaseDtoB : ICaseDto {
    public int Id { get; set; }
    public string Type => "B";
    public string ExtraB { get; set; } = string.Empty;
}

public class CaseDtoC : ICaseDto {
    public int Id { get; set; }
    public string Type => "C";
    public string ExtraC { get; set; } = string.Empty;
}
