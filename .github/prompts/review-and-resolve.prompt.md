---
mode: agent
---

# Code Review and Resolution Prompt

## Task Definition
Review the codebase for compliance with established coding standards and resolve any violations found. Focus on high-impact improvements that enhance code quality, maintainability, and adherence to project conventions.

## Requirements

### 1. **Review Against Instruction Files**
Analyze code compliance with all instruction files in `.github/instructions/`:

- `aspnet-core.instructions.md` - ASP.NET Core patterns and best practices
- `csharp-coding.instructions.md` - C# coding standards and conventions
- `testing-strategy.instructions.md` - Testing framework usage and patterns
- `razor-views.instructions.md` - View structure and accessibility guidelines
- `documentation.instructions.md` - Documentation standards and requirements
- `frameworks-and-libraries.instructions.md` - Approved technology stack compliance

### 2. **Code Quality Focus Areas**

**High Priority Violations:**
- Security and performance issues
- Critical violations of instruction file guidelines
- Breaking changes to established patterns

**Medium Priority Improvements:**
- Missing documentation or tests
- Inconsistencies with project conventions
- Code quality improvements

**Low Priority (Skip for this review):**
- Minor formatting issues handled by .editorconfig
- Cosmetic improvements without functional impact

### 3. **Resolution Strategy**

**For Each Violation Found:**
1. **Categorize Impact** - High/Medium/Low based on security, performance, and maintainability
2. **Provide Specific Fix** - Include exact code changes with before/after examples
3. **Reference Guidelines** - Cite specific instruction file and section
4. **Validate Changes** - Ensure fixes align with project patterns and don't introduce regressions

**Implementation Approach:**
- Make focused, atomic changes - one logical improvement per file edit
- Follow established patterns from existing codebase
- Maintain backward compatibility unless explicitly breaking changes are needed
- Add appropriate tests for any new functionality introduced

## Success Criteria

### ✅ **Compliance Achieved**
- All high-priority violations from instruction files resolved
- Code adheres to established project patterns and conventions
- No breaking changes to existing functionality

### ✅ **Validation Passed**
Execute validation sequence from `dotnet-commands.instructions.md`:

**Expected Results:**
- Clean build with zero warnings
- All tests passing
- Improved code maintainability

## Constraints

### **Scope Limitations**
- **Focus on high-confidence changes only** - avoid modifications where requirements are ambiguous
- **Preserve existing functionality** - ensure no behavioral changes unless explicitly required
- **Maintain API compatibility** - don't break existing public interfaces

### **Change Guidelines**
- **One logical change per file edit** - keep modifications focused and reviewable
- **Include context in changes** - provide 3-5 lines of surrounding code for clarity
- **Test after each change** - validate that modifications don't introduce regressions
- **Reference instruction files** - cite specific guidelines when making changes
