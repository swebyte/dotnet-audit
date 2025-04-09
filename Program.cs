using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<AuditInterceptor>();

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) => {
    options.UseSqlite("Data Source=cases.db")
           .AddInterceptors(serviceProvider.GetRequiredService<AuditInterceptor>());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserContextService, UserContextService>();

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

app.MapPost("/casetypea", async (AppDbContext db, CaseTypeA newCaseTypeA, string userid, string fullname, IUserContextService userContextService) => {
    // Set user info in the UserContextService
    userContextService.UserId = userid;
    userContextService.Fullname = fullname;

    var newCase = new Case { Type = "A", CaseTypeA = newCaseTypeA };
    db.Cases.Add(newCase);
    await db.SaveChangesAsync();
    return Results.Created($"/casetypea/{newCase.CaseTypeA.Id}", newCase.CaseTypeA);
});

app.MapPut("/casetypea/{id}", async (AppDbContext db, int id, string newExtraA, string status, string userid, string fullname, IUserContextService userContextService) => {
    // Set user info in the UserContextService
    userContextService.UserId = userid;
    userContextService.Fullname = fullname;

    var caseTypeA = await db.CaseTypeAs.Include(c => c.Case).FirstOrDefaultAsync(c => c.CaseId == id);
    if (caseTypeA == null) {
        return Results.NotFound();
    }

    caseTypeA.ExtraA = newExtraA;
    caseTypeA.Status = status;
    await db.SaveChangesAsync();
    return Results.Ok(new CaseDtoA() { 
        Id = caseTypeA.CaseId, 
        ExtraA = caseTypeA.ExtraA 
    });
});

app.Run();

void SeedDatabase(AppDbContext db) {
    if (!db.Cases.Any()) {
        var cases = new List<Case>();

        for (int i = 1; i <= 10; i++) {
            cases.Add(new Case { Type = "A", CaseTypeA = new CaseTypeA { ExtraA = $"Extra Data A {i}", Status = "New" } });
            cases.Add(new Case { Type = "B", CaseTypeB = new CaseTypeB { ExtraB = $"Extra Data B {i}" } });
            cases.Add(new Case { Type = "C", CaseTypeC = new CaseTypeC { ExtraC = $"Extra Data C {i}" } });
        }

        db.Cases.AddRange(cases);
        db.SaveChanges();
    }
}
