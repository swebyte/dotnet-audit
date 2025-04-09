using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options => {
    options.UseSqlite("Data Source=cases.db"); // Keep DbContext configuration simple
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JSON serialization to handle reference loops
builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

var app = builder.Build();

// Ensure the database schema is created and migrations are applied
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // Apply migrations
    SeedDatabase(db);
}

app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Case API V1");
    c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
});

app.MapGet("/", () => "Hello World!");

// Update the /cases endpoint to include navigation properties in the response
app.MapGet("/cases", async (AppDbContext db) => {
    var cases = new List<ICaseDto>();
    var dbCases = await db.Cases
        .Include(c => c.CaseTypeA)
        .Include(c => c.CaseTypeB)
        .Include(c => c.CaseTypeC)
        .ToListAsync();

    foreach (var c in dbCases) {
        ICaseDto caseDto = c.Type switch {
            "A" => new CaseDtoA { Id = c.Id, ExtraA = c.CaseTypeA!.ExtraA },
            "B" => new CaseDtoB { Id = c.Id, ExtraB = c.CaseTypeB!.ExtraB },
            "C" => new CaseDtoC { Id = c.Id, ExtraC = c.CaseTypeC!.ExtraC },
            _ => throw new InvalidOperationException("Unknown case type")
        };
        cases.Add(caseDto);
    }

    return Results.Ok(cases);
});

app.MapGet("/audits", async (AppDbContext db) => {
    var audits = await db.CaseAudits.ToListAsync();
    return Results.Ok(audits);
});

app.MapPost("/casetypea", async (AppDbContext db, CaseTypeA newCaseTypeA) => {
    var newCase = new Case { Type = "A", CaseTypeA = newCaseTypeA };
    db.Cases.Add(newCase);
    await db.SaveChangesAsync();
    return Results.Created($"/casetypea/{newCase.CaseTypeA.Id}", newCase.CaseTypeA);
});

app.MapPut("/casetypea/{id}", async (AppDbContext db, int id, string newExtraA) => {
    var caseTypeA = await db.CaseTypeAs.FindAsync(id);
    if (caseTypeA == null) {
        return Results.NotFound();
    }

    caseTypeA.ExtraA = newExtraA;
    await db.SaveChangesAsync();
    return Results.Ok(caseTypeA);
});

app.Run();

void SeedDatabase(AppDbContext db) {
    if (!db.Cases.Any()) {
        var caseA = new Case { Type = "A", CaseTypeA = new CaseTypeA { ExtraA = "Extra Data A" } };
        var caseB = new Case { Type = "B", CaseTypeB = new CaseTypeB { ExtraB = "Extra Data B" } };
        var caseC = new Case { Type = "C", CaseTypeC = new CaseTypeC { ExtraC = "Extra Data C" } };

        db.Cases.AddRange(caseA, caseB, caseC);
        db.SaveChanges();
    }
}
