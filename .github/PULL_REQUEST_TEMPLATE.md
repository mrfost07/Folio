## Note for external contributors

Folio is not merging external pull requests at this stage — please see [CONTRIBUTING.md](../CONTRIBUTING.md). Bug reports, ideas, and discussions are very welcome via Issues / Discussions.

---

## Maintainer checklist

- [ ] dotnet test .	ests\Folio.Tests\Folio.Tests.csproj --no-restore --verbosity:minimal -p:Platform=x64 all green
- [ ] Serialization/boundary changes → ratchet updated on both layers (baseline count + AotStage literals)
- [ ] FOLIO_NATIVE_AOT files touched → AOT retail publish verified, packages.lock.json restored after
- [ ] Commit messages carry only Mark Renier B. Fostanes <mfostanes@ssct.edu.ph> — no Co-Authored-By / generated-with trailers (hook + CI enforce)
- [ ] Docs/version numbers updated if user-visible (changelog, READMEs, freeze counts for UI contract tests)
